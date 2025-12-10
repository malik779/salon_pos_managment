import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import '../../core/models/domain_models.dart';
import '../../core/providers/app_providers.dart';
import '../../core/services/domain_api.dart';
import '../../core/services/sync_service.dart';

class BookingPage extends ConsumerWidget {
  const BookingPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final bookings = ref.watch(bookingProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Booking Calendar')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _createQuickBooking(context, ref),
        label: const Text('New Booking'),
        icon: const Icon(Icons.add),
      ),
      body: bookings.when(
        data: (slots) => ListView.builder(
          padding: const EdgeInsets.all(16),
          itemCount: slots.length,
          itemBuilder: (context, index) => _BookingCard(slot: slots[index]),
        ),
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Failed to load calendar: $err')),
      ),
    );
  }

  Future<void> _createQuickBooking(BuildContext context, WidgetRef ref) async {
    final payload = {
      'branchId': '00000000-0000-0000-0000-000000000001',
      'clientId': '00000000-0000-0000-0000-000000000002',
      'staffId': '00000000-0000-0000-0000-000000000003',
      'startUtc': DateTime.now().toUtc().toIso8601String(),
      'endUtc': DateTime.now().add(const Duration(minutes: 60)).toUtc().toIso8601String(),
    };
    final bookingApi = ref.read(bookingApiProvider);
    final sync = ref.read(syncServiceProvider);
    try {
      await bookingApi.create(payload);
    } catch (_) {
      await sync.enqueueOperation('CreateBooking', payload);
    }
    if (context.mounted) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Booking queued for sync')));
    }
  }
}

class _BookingCard extends StatelessWidget {
  const _BookingCard({required this.slot});

  final BookingSlot slot;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        title: Text('${slot.startUtc.hour}:${slot.startUtc.minute.toString().padLeft(2, '0')}'),
        subtitle: Text(slot.status),
        trailing: Text(slot.staffId.substring(0, 4)),
      ),
    );
  }
}
