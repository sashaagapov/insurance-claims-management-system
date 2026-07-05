import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { environment } from '../../../environments/environment';
import { AuthContextService } from '../services/auth-context.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  if (!request.url.startsWith(environment.apiBaseUrl)) {
    return next(request);
  }

  const authContext = inject(AuthContextService);
  const activeUser = authContext.activeUser();

  return next(
    request.clone({
      setHeaders: {
        'X-User-Id': activeUser.id,
      },
    }),
  );
};
