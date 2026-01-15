import 'package:equatable/equatable.dart';

class Branch extends Equatable {
  const Branch({required this.id, required this.name, required this.timezone, required this.address});

  factory Branch.fromJson(Map<String, dynamic> json) => Branch(
        id: json['id'] as String,
        name: json['name'] as String,
        timezone: json['timezone'] as String,
        address: json['address'] as String? ?? '',
      );

  final String id;
  final String name;
  final String timezone;
  final String address;

  Map<String, dynamic> toJson() => {
        'id': id,
        'name': name,
        'timezone': timezone,
        'address': address,
      };

  @override
  List<Object?> get props => [id, name, timezone, address];
}

class Staff extends Equatable {
  const Staff({required this.id, required this.fullName, required this.role, required this.defaultBranchId});

  factory Staff.fromJson(Map<String, dynamic> json) => Staff(
        id: json['id'] as String,
        fullName: json['fullName'] as String,
        role: json['role'] as String,
        defaultBranchId: json['defaultBranchId'] as String,
      );

  final String id;
  final String fullName;
  final String role;
  final String defaultBranchId;

  @override
  List<Object?> get props => [id, fullName, role, defaultBranchId];
}

class Client extends Equatable {
  const Client({required this.id, required this.firstName, required this.lastName, required this.phone, this.email});

  factory Client.fromJson(Map<String, dynamic> json) => Client(
        id: json['id'] as String,
        firstName: json['firstName'] as String,
        lastName: json['lastName'] as String,
        phone: json['phone'] as String,
        email: json['email'] as String?,
      );

  final String id;
  final String firstName;
  final String lastName;
  final String phone;
  final String? email;

  @override
  List<Object?> get props => [id, firstName, lastName, phone, email];
}

class BookingSlot extends Equatable {
  const BookingSlot({
    required this.id,
    required this.branchId,
    required this.staffId,
    required this.clientId,
    required this.startUtc,
    required this.endUtc,
    required this.status,
  });

  factory BookingSlot.fromJson(Map<String, dynamic> json) => BookingSlot(
        id: json['id'] as String,
        branchId: json['branchId'] as String,
        staffId: json['staffId'] as String,
        clientId: json['clientId'] as String,
        startUtc: DateTime.parse(json['startUtc'] as String),
        endUtc: DateTime.parse(json['endUtc'] as String),
        status: json['status'] as String,
      );

  final String id;
  final String branchId;
  final String staffId;
  final String clientId;
  final DateTime startUtc;
  final DateTime endUtc;
  final String status;

  Map<String, dynamic> toJson() => {
        'id': id,
        'branchId': branchId,
        'staffId': staffId,
        'clientId': clientId,
        'startUtc': startUtc.toIso8601String(),
        'endUtc': endUtc.toIso8601String(),
        'status': status,
      };

  @override
  List<Object?> get props => [id, branchId, staffId, clientId, startUtc, endUtc, status];
}

class CartLine extends Equatable {
  const CartLine({required this.itemId, required this.itemType, required this.quantity, required this.unitPrice});

  final String itemId;
  final String itemType;
  final int quantity;
  final double unitPrice;

  Map<String, dynamic> toJson() => {
        'itemId': itemId,
        'itemType': itemType,
        'quantity': quantity,
        'unitPrice': unitPrice,
      };

  @override
  List<Object?> get props => [itemId, itemType, quantity, unitPrice];
}
