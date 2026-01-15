import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/models/domain_models.dart';
import 'package:salon_pos_mobile/core/services/domain_api.dart';

class StaffPage extends ConsumerWidget {
  const StaffPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final staffFuture = ref.watch(_staffProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Staff & Commissions')),
      body: staffFuture.when(
        data: (staff) => ListView.builder(
          itemCount: staff.length,
          itemBuilder: (context, index) {
            final member = staff[index];
            return ListTile(
              title: Text(member.fullName),
              subtitle: Text(member.role),
            );
          },
        ),
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Error: $err')),
      ),
    );
  }
}

final _staffProvider = FutureProvider<List<Staff>>((ref) async {
  final api = ref.watch(staffApiProvider);
  return api.list();
});
