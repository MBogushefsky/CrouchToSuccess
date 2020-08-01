import { TestBed } from '@angular/core/testing';

import { YodleeService } from './yodlee.service';

describe('YodleeService', () => {
  let service: YodleeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(YodleeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
