import 'dart:convert';

import 'package:path/path.dart' as p;
import 'package:path_provider/path_provider.dart';
import 'package:sqflite/sqflite.dart';
import 'package:uuid/uuid.dart';

class OfflineQueue {
  OfflineQueue._(this._db);

  static const _table = 'operations';
  final Database _db;

  static Future<OfflineQueue> open() async {
    final dir = await getApplicationDocumentsDirectory();
    final dbPath = p.join(dir.path, 'offline_queue.db');
    final db = await openDatabase(dbPath, version: 1, onCreate: (db, version) async {
      await db.execute('''
      CREATE TABLE IF NOT EXISTS $_table (
        id TEXT PRIMARY KEY,
        type TEXT NOT NULL,
        payload TEXT NOT NULL,
        createdAt INTEGER NOT NULL
      )
      ''');
    },);
    return OfflineQueue._(db);
  }

  Future<String> enqueue(String type, Map<String, dynamic> payload) async {
    final id = const Uuid().v4();
    await _db.insert(_table, {
      'id': id,
      'type': type,
      'payload': jsonEncode(payload),
      'createdAt': DateTime.now().millisecondsSinceEpoch,
    });
    return id;
  }

  Future<List<Map<String, dynamic>>> pending() async {
    final rows = await _db.query(_table, orderBy: 'createdAt ASC');
    return rows
        .map((row) => {
              'id': row['id'],
              'type': row['type'],
              'payload': jsonDecode(row['payload']! as String) as Map<String, dynamic>,
            },)
        .toList();
  }

  Future<void> remove(String id) async {
    await _db.delete(_table, where: 'id = ?', whereArgs: [id]);
  }
}
