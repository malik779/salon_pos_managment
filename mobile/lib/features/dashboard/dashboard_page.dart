import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/providers/app_providers.dart';
import 'package:salon_pos_mobile/core/services/sync_service.dart';

class DashboardPage extends ConsumerWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final branches = ref.watch(branchesProvider);
    final syncService = ref.read(syncServiceProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('Salon Dashboard'),
        actions: [
          IconButton(onPressed: () => Navigator.pushNamed(context, '/settings'), icon: const Icon(Icons.settings)),
        ],
      ),
      drawer: const _AppDrawer(),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            StreamBuilder<SyncStatus>(
              stream: syncService.statusStream,
              builder: (context, snapshot) {
                final status = snapshot.data ?? SyncStatus.online;
                return Chip(label: Text('Sync: ${status.name}'));
              },
            ),
            const SizedBox(height: 16),
            branches.when(
              data: (data) => Wrap(
                spacing: 16,
                children: data
                    .map((branch) => Card(
                          child: Padding(
                            padding: const EdgeInsets.all(16),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(branch.name, style: Theme.of(context).textTheme.titleMedium),
                                Text(branch.timezone),
                              ],
                            ),
                          ),
                        ),)
                    .toList(),
              ),
              error: (err, _) => Text('Failed to load branches: $err'),
              loading: () => const LinearProgressIndicator(),
            ),
          ],
        ),
      ),
    );
  }
}

class _AppDrawer extends StatelessWidget {
  const _AppDrawer();

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        children: [
          const DrawerHeader(child: Text('Navigation')),
          _navTile(context, 'Dashboard', '/'),
          _navTile(context, 'Booking', '/booking'),
          _navTile(context, 'POS', '/pos'),
          _navTile(context, 'Clients', '/clients'),
          _navTile(context, 'Staff', '/staff'),
        ],
      ),
    );
  }

  ListTile _navTile(BuildContext context, String title, String route) => ListTile(
        title: Text(title),
        onTap: () => Navigator.pushReplacementNamed(context, route),
      );
}
