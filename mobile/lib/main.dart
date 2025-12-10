import 'bootstrap.dart';
import 'app.dart';

Future<void> main() async {
  await bootstrap(() => const SalonApp());
}
