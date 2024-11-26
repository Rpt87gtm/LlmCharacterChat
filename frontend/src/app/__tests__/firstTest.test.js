import { expect } from 'chai';

describe('Mocha Test Feature', () => {
  it('should test successfully', () => {
    const result = "test";
    expect(result).to.equal('test');
  });
});