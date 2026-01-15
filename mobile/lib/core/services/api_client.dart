import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/env.dart';

final dioProvider = Provider<Dio>((ref) {
  final dio = Dio(BaseOptions(baseUrl: Env.apiBaseUrl, connectTimeout: const Duration(seconds: 10)));
  dio.interceptors.add(QueuedInterceptorsWrapper(
    onRequest: (options, handler) async {
      const storage = FlutterSecureStorage();
      final token = await storage.read(key: 'access_token');
      if (token != null && token.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $token';
      }
      options.headers.putIfAbsent('Idempotency-Key', () => options.headers['Idempotency-Key'] ?? DateTime.now().microsecondsSinceEpoch.toString());
      return handler.next(options);
    },
    onError: (error, handler) {
      // TODO: integrate retry policy.
      return handler.next(error);
    },
  ),);
  return dio;
});
