import 'package:bluetooth_print/bluetooth_print.dart';
import 'package:bluetooth_print/bluetooth_print_model.dart';

class PrintingService {
  PrintingService(this._printer);

  final BluetoothPrint _printer;

  Future<void> printReceipt(String title, List<Map<String, String>> lines) async {
    final isConnected = await _printer.isConnected ?? false;
    if (!isConnected) {
      final devices = await _printer.startScan(timeout: const Duration(seconds: 4));
      if (devices != null && devices.isNotEmpty) {
        await _printer.connect(devices.first);
      }
    }
    final items = <LineText>[
      LineText(content: title, weight: 2, width: 2, height: 2, align: LineTextAlign.center),
      ...lines.map((line) => LineText(content: '${line['label']}: ${line['value']}')),
    ];
    await _printer.printReceipt(config: {}, data: items);
  }
}
