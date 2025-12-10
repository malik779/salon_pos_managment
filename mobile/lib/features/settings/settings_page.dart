import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import '../../core/services/sync_service.dart';

class SettingsPage extends ConsumerStatefulWidget {
  const SettingsPage({super.key});

  @override
  ConsumerState<SettingsPage> createState() => _SettingsPageState();
}

class _SettingsPageState extends ConsumerState<SettingsPage> {
  bool offlineMode = true;
  bool bluetoothReceipt = false;

  @override
  Widget build(BuildContext context) {
    final syncService = ref.watch(syncServiceProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Settings')),
      body: ListView(
        children: [
          SwitchListTile(
            title: const Text('Offline mode'),
            subtitle: StreamBuilder<SyncStatus>(
              stream: syncService.statusStream,
              builder: (context, snapshot) => Text('Status: ${snapshot.data?.name ?? 'online'}'),
            ),
            value: offlineMode,
            onChanged: (value) => setState(() => offlineMode = value),
          ),
          SwitchListTile(
            title: const Text('Bluetooth receipts'),
            value: bluetoothReceipt,
            onChanged: (value) => setState(() => bluetoothReceipt = value),
          ),
        ],
      ),
    );
  }
}
