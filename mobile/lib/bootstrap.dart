import 'dart:async';

import 'package:flutter/widgets.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/services/sync_service.dart';

Future<void> bootstrap(FutureOr<Widget> Function() builder) async {
  WidgetsFlutterBinding.ensureInitialized();
  final container = ProviderContainer();
  final syncService = container.read(syncServiceProvider);
  await syncService.startBackgroundSync();
  runApp(UncontrolledProviderScope(container: container, child: await builder()));
}
