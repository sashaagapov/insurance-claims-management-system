import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home').then((component) => component.Home),
  },
  {
    path: 'claims',
    loadComponent: () =>
      import('./features/placeholder-page/placeholder-page').then(
        (component) => component.PlaceholderPage,
      ),
    data: {
      title: 'Claims',
      description: 'The claims dashboard will be implemented in a later approved phase.',
    },
  },
  {
    path: 'claims/new',
    loadComponent: () =>
      import('./features/placeholder-page/placeholder-page').then(
        (component) => component.PlaceholderPage,
      ),
    data: {
      title: 'New Claim',
      description: 'The FNOL create claim form will be implemented in a later approved phase.',
    },
  },
];
