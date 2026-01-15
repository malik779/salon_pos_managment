import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';
import { BranchStore } from '@app/branches/store/branch.store';
import { RegisterComponent } from './register.component';
import { IdentityApi } from '../core/api/api-client';
import { Branch } from '../core/models/domain.models';

describe('RegisterComponent', () => {
  let fixture: ComponentFixture<RegisterComponent>;
  let component: RegisterComponent;
  let identityApiMock: { register: jest.Mock };
  let routerMock: { navigate: jest.Mock };
  let branchStoreMock: { loadAll: jest.Mock; allItems: ReturnType<typeof signal<Branch[]>> };

  const branch: Branch = {
    id: 'branch-1',
    salonId: 'salon-1',
    name: 'Downtown',
    code: 'DT',
    timezone: 'UTC',
    address: '123 Main St',
    phone: '555-0100',
    email: 'downtown@salon.test'
  };

  beforeEach(async () => {
    identityApiMock = {
      register: jest.fn().mockReturnValue(of({} as any))
    };
    routerMock = {
      navigate: jest.fn()
    };
    branchStoreMock = {
      loadAll: jest.fn(),
      allItems: signal([branch])
    };

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, NoopAnimationsModule],
      providers: [
        { provide: IdentityApi, useValue: identityApiMock },
        { provide: BranchStore, useValue: branchStoreMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('syncs salonId when branch is selected', () => {
    component.form.controls.branchId.setValue(branch.id);
    fixture.detectChanges();

    expect(branchStoreMock.loadAll).toHaveBeenCalled();
    expect(component.form.controls.salonId.value).toBe(branch.salonId);
  });

  it('submits registration with salonId from branch', () => {
    component.form.patchValue({
      fullName: 'Jordan Stylist',
      email: 'jordan@example.com',
      password: 'Password1',
      confirmPassword: 'Password1',
      branchId: branch.id,
      role: 'Staff'
    });

    component.submit();

    expect(identityApiMock.register).toHaveBeenCalledWith(
      'jordan@example.com',
      'Password1',
      'Jordan Stylist',
      'salon-1',
      'branch-1',
      'Staff'
    );
    expect(routerMock.navigate).toHaveBeenCalledWith(['/auth/login']);
  });
});
