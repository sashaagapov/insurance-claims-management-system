import { Injectable, computed, signal } from '@angular/core';

import { MockUser } from '../models';

@Injectable({
  providedIn: 'root',
})
export class AuthContextService {
  readonly users: readonly MockUser[] = [
    {
      id: 'bbbbbbbb-0000-0000-0000-000000000001',
      displayName: 'Handler',
      role: 'Handler',
    },
    {
      id: 'bbbbbbbb-0000-0000-0000-000000000002',
      displayName: 'Supervisor',
      role: 'Supervisor',
    },
    {
      id: 'bbbbbbbb-0000-0000-0000-000000000003',
      displayName: 'Manager',
      role: 'Manager',
    },
  ];

  private readonly activeUserId = signal(this.users[0].id);

  readonly activeUser = computed(() => {
    const activeId = this.activeUserId();
    return this.users.find((user) => user.id === activeId) ?? this.users[0];
  });

  setActiveUser(userId: string): void {
    if (this.users.some((user) => user.id === userId)) {
      this.activeUserId.set(userId);
    }
  }
}
