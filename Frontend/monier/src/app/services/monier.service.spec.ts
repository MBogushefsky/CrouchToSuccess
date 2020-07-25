import { TestBed } from '@angular/core/testing';

import { MonierService } from './monier.service';

describe('MonierService', () => {
  let service: MonierService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MonierService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
