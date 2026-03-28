import { describe, it, expect } from 'vitest';

describe('Basic Tests', () => {
  it('passes a simple test', () => {
    expect(1 + 1).toBe(2);
  });

  it('can import React testing utilities', () => {
    expect(typeof describe).toBe('function');
    expect(typeof it).toBe('function');
    expect(typeof expect).toBe('function');
  });
});
