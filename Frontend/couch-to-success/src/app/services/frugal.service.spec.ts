import { TestBed } from '@angular/core/testing';

import { FrugalService } from './frugal.service';

describe('FrugalService', () => {
  let service: FrugalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FrugalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
