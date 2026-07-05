export type MockUserRole = 'Handler' | 'Supervisor' | 'Manager';

export interface MockUser {
  id: string;
  displayName: string;
  role: MockUserRole;
}
