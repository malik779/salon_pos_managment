import 'package:salon_pos_mobile/app.dart';
import 'package:salon_pos_mobile/bootstrap.dart';

Future<void> main() async {
  await bootstrap(() => const SalonApp());
}
