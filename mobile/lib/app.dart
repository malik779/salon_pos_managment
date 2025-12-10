import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'features/auth/login_page.dart';
import 'features/dashboard/dashboard_page.dart';
import 'features/booking/booking_page.dart';
import 'features/pos/pos_page.dart';
import 'features/clients/clients_page.dart';
import 'features/staff/staff_page.dart';
import 'features/settings/settings_page.dart';

class SalonApp extends ConsumerWidget {
  const SalonApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final baseTheme = ThemeData(
      colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF2563EB)),
      textTheme: GoogleFonts.interTextTheme(),
      useMaterial3: true,
    );

    return MaterialApp(
      title: 'Salon POS',
      theme: baseTheme,
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      supportedLocales: const [Locale('en')],
      routes: {
        '/': (_) => const DashboardPage(),
        '/booking': (_) => const BookingPage(),
        '/pos': (_) => const PosPage(),
        '/clients': (_) => const ClientsPage(),
        '/staff': (_) => const StaffPage(),
        '/settings': (_) => const SettingsPage(),
        '/auth/login': (_) => const LoginPage(),
      },
      initialRoute: '/',
    );
  }
}
