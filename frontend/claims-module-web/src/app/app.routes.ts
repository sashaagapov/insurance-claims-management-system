import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'claims',
  },
  {
    path: 'claims',
    loadComponent: () =>
      import('./features/claims/claims-list/claims-list').then((component) => component.ClaimsList),
  },
  {
    path: 'claims/new',
    loadComponent: () =>
      import('./features/claims/fnol-form/fnol-form').then((component) => component.FnolForm),
  },
  {
    path: 'claims/:id',
    loadComponent: () =>
      import('./features/placeholder-page/placeholder-page').then(
        (component) => component.PlaceholderPage,
      ),
    data: {
      title: 'Claim Detail',
      description: 'The claim detail screen will be implemented in a later approved phase.',
    },
  },
];
