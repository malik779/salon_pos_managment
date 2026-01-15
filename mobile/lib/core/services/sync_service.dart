import 'dart:async';

import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:dio/dio.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/storage/offline_queue.dart';
import 'package:salon_pos_mobile/env.dart';

final syncServiceProvider = Provider<SyncService>((ref) {
  final dio = Dio(BaseOptions(baseUrl: Env.syncServiceBase));
  return SyncService(dio, queueFuture: OfflineQueue.open());
});

enum SyncStatus { online, syncing, offline }

class SyncService {
  SyncService(this._dio, {required Future<OfflineQueue> queueFuture}) : _queueFuture = queueFuture;

  final Dio _dio;
  final Future<OfflineQueue> _queueFuture;
  final _statusController = StreamController<SyncStatus>.broadcast();

  Stream<SyncStatus> get statusStream => _statusController.stream;

  Future<void> startBackgroundSync() async {
    _statusController.add(SyncStatus.online);
    Connectivity().onConnectivityChanged.listen((result) {
      if (result == ConnectivityResult.none) {
        _statusController.add(SyncStatus.offline);
      } else {
        _statusController.add(SyncStatus.syncing);
        unawaited(_flushQueue());
      }
    });
    unawaited(_flushQueue());
  }

  Future<String> enqueueOperation(String type, Map<String, dynamic> payload) async {
    final queue = await _queueFuture;
    final id = await queue.enqueue(type, payload);
    unawaited(_flushQueue());
    return id;
  }

  Future<void> _flushQueue() async {
    final queue = await _queueFuture;
    final ops = await queue.pending();
    if (ops.isEmpty) {
      _statusController.add(SyncStatus.online);
      return;
    }
    _statusController.add(SyncStatus.syncing);
    for (final op in ops) {
      try {
        await _dio.post('/sync/push', data: {
          'deviceId': 'mobile',
          'syncToken': 'local-dev',
          'operations': [
            {
              'sequence': DateTime.now().microsecondsSinceEpoch,
              'operationType': op['type'],
              'payload': op['payload'],
            }
          ],
        },);
        await queue.remove(op['id'] as String);
      } on DioException {
        _statusController.add(SyncStatus.offline);
        break;
      }
    }
    _statusController.add(SyncStatus.online);
  }
}
