import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/models/domain_models.dart';
import 'package:salon_pos_mobile/core/services/domain_api.dart';

final branchesProvider = FutureProvider<List<Branch>>((ref) async {
  final api = ref.watch(branchApiProvider);
  return api.list();
});

final staffProvider = FutureProvider<List<Staff>>((ref) async {
  final api = ref.watch(staffApiProvider);
  return api.list();
});

final bookingProvider = FutureProvider<List<BookingSlot>>((ref) async {
  final api = ref.watch(bookingApiProvider);
  final now = DateTime.now().toUtc();
  final to = now.add(const Duration(hours: 12));
  return api.calendar('00000000-0000-0000-0000-000000000001', now, to);
});
