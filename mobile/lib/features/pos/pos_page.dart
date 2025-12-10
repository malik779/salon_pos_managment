import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:uuid/uuid.dart';
import '../../core/models/domain_models.dart';
import '../../core/services/domain_api.dart';
import '../../core/services/sync_service.dart';

class PosPage extends ConsumerStatefulWidget {
  const PosPage({super.key});

  @override
  ConsumerState<PosPage> createState() => _PosPageState();
}

class _PosPageState extends ConsumerState<PosPage> {
  final _formKey = GlobalKey<FormState>();
  final List<CartLine> _lines = [];
  final _skuController = TextEditingController();
  final _qtyController = TextEditingController(text: '1');
  final _priceController = TextEditingController(text: '0');

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('POS Register')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            Form(
              key: _formKey,
              child: Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _skuController,
                      decoration: const InputDecoration(labelText: 'SKU'),
                      validator: (value) => (value == null || value.isEmpty) ? 'Required' : null,
                    ),
                  ),
                  const SizedBox(width: 12),
                  SizedBox(
                    width: 80,
                    child: TextFormField(
                      controller: _qtyController,
                      decoration: const InputDecoration(labelText: 'Qty'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 12),
                  SizedBox(
                    width: 120,
                    child: TextFormField(
                      controller: _priceController,
                      decoration: const InputDecoration(labelText: 'Price'),
                      keyboardType: const TextInputType.numberWithOptions(decimal: true),
                    ),
                  ),
                  const SizedBox(width: 12),
                  FilledButton(onPressed: _addLine, child: const Text('Add')),
                ],
              ),
            ),
            const SizedBox(height: 24),
            Expanded(
              child: ListView.builder(
                itemCount: _lines.length,
                itemBuilder: (context, index) {
                  final line = _lines[index];
                  return ListTile(
                    title: Text('${line.itemId} x${line.quantity}'),
                    trailing: Text('\$${line.unitPrice.toStringAsFixed(2)}'),
                  );
                },
              ),
            ),
            FilledButton.icon(onPressed: _lines.isEmpty ? null : _checkout, icon: const Icon(Icons.point_of_sale), label: const Text('Finalize Invoice')),
          ],
        ),
      ),
    );
  }

  void _addLine() {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _lines.add(CartLine(
        itemId: _skuController.text,
        itemType: 'product',
        quantity: int.tryParse(_qtyController.text) ?? 1,
        unitPrice: double.tryParse(_priceController.text) ?? 0,
      ));
    });
    _skuController.clear();
    _qtyController.text = '1';
    _priceController.text = '0';
  }

  Future<void> _checkout() async {
    final posApi = ref.read(posApiProvider);
    final payload = {
      'invoiceId': const Uuid().v4(),
      'branchId': '00000000-0000-0000-0000-000000000001',
      'clientId': '00000000-0000-0000-0000-000000000002',
      'lines': _lines.map((line) => line.toJson()).toList(),
      'discount': 0,
    };
    final syncService = ref.read(syncServiceProvider);
    try {
      await posApi.createInvoice(payload);
    } catch (_) {
      await syncService.enqueueOperation('InvoiceCreated', payload);
    } finally {
      await syncService.enqueueOperation('InvoiceSnapshot', payload);
    }
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Invoice submitted')));
      setState(() => _lines.clear());
    }
  }
}
