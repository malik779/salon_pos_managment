import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'domain_api.dart';

final authServiceProvider = Provider<AuthService>((ref) {
  final identityApi = ref.watch(identityApiProvider);
  return AuthService(identityApi);
});

class AuthService {
  AuthService(this._identityApi);

  final IdentityApi _identityApi;
  final _storage = const FlutterSecureStorage();

  Future<bool> login(String username, String password) async {
    final data = await _identityApi.login(username, password);
    await _storage.write(key: 'access_token', value: data['accessToken'] as String);
    await _storage.write(key: 'refresh_token', value: data['refreshToken'] as String);
    return true;
  }

  Future<void> logout() async {
    await _storage.deleteAll();
  }

  Future<String?> get token async => _storage.read(key: 'access_token');
}
