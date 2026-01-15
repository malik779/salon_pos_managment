import 'package:dio/dio.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:salon_pos_mobile/core/models/domain_models.dart';
import 'package:salon_pos_mobile/core/services/api_client.dart';

final identityApiProvider = Provider<IdentityApi>((ref) => IdentityApi(ref.watch(dioProvider)));
final bookingApiProvider = Provider<BookingApi>((ref) => BookingApi(ref.watch(dioProvider)));
final posApiProvider = Provider<PosApi>((ref) => PosApi(ref.watch(dioProvider)));
final clientApiProvider = Provider<ClientApi>((ref) => ClientApi(ref.watch(dioProvider)));
final staffApiProvider = Provider<StaffApi>((ref) => StaffApi(ref.watch(dioProvider)));
final branchApiProvider = Provider<BranchApi>((ref) => BranchApi(ref.watch(dioProvider)));

String _serviceBase(int port) => 'http://localhost:$port';

class IdentityApi {
  IdentityApi(this._dio);
  final Dio _dio;

  Future<Map<String, dynamic>> login(String username, String password) async {
    final response = await _dio.post('${_serviceBase(5001)}/auth/token', data: {
      'username': username,
      'password': password,
    },);
    return response.data as Map<String, dynamic>;
  }
}

class BranchApi {
  BranchApi(this._dio);
  final Dio _dio;

  Future<List<Branch>> list() async {
    final response = await _dio.get('${_serviceBase(5003)}/branches');
    return (response.data as List<dynamic>).map((e) => Branch.fromJson(e as Map<String, dynamic>)).toList();
  }
}

class BookingApi {
  BookingApi(this._dio);
  final Dio _dio;

  Future<List<BookingSlot>> calendar(String branchId, DateTime from, DateTime to) async {
    final response = await _dio.get('${_serviceBase(5002)}/calendar', queryParameters: {
      'branchId': branchId,
      'from': from.toIso8601String(),
      'to': to.toIso8601String(),
    },);
    return (response.data as List<dynamic>).map((e) => BookingSlot.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<void> create(Map<String, dynamic> payload) async {
    await _dio.post('${_serviceBase(5002)}/bookings', data: payload);
  }
}

class PosApi {
  PosApi(this._dio);
  final Dio _dio;

  Future<void> createInvoice(Map<String, dynamic> payload) async {
    await _dio.post('${_serviceBase(5005)}/invoices', data: payload);
  }
}

class ClientApi {
  ClientApi(this._dio);
  final Dio _dio;

  Future<List<Client>> list() async {
    final response = await _dio.get('${_serviceBase(5008)}/clients');
    return (response.data as List<dynamic>).map((e) => Client.fromJson(e as Map<String, dynamic>)).toList();
  }
}

class StaffApi {
  StaffApi(this._dio);
  final Dio _dio;

  Future<List<Staff>> list() async {
    final response = await _dio.get('${_serviceBase(5007)}/staff');
    return (response.data as List<dynamic>).map((e) => Staff.fromJson(e as Map<String, dynamic>)).toList();
  }
}
