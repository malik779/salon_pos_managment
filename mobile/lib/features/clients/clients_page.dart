import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/models/domain_models.dart';
import 'package:salon_pos_mobile/core/services/domain_api.dart';

class ClientsPage extends ConsumerWidget {
  const ClientsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final clientsFuture = ref.watch(_clientsProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Clients')),
      body: clientsFuture.when(
        data: (clients) => ListView.builder(
          itemCount: clients.length,
          itemBuilder: (context, index) {
            final client = clients[index];
            return ListTile(
              title: Text('${client.firstName} ${client.lastName}'),
              subtitle: Text(client.phone),
            );
          },
        ),
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Error: $err')),
      ),
    );
  }
}

final _clientsProvider = FutureProvider<List<Client>>((ref) async {
  final api = ref.watch(clientApiProvider);
  return api.list();
});
