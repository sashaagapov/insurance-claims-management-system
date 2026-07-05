import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';

interface ApiErrorBody {
  error?: string;
  message?: string;
  title?: string;
}

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const snackBar = inject(MatSnackBar);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && request.url.startsWith(environment.apiBaseUrl)) {
        snackBar.open(getApiErrorMessage(error), 'Dismiss', {
          duration: 5000,
          verticalPosition: 'top',
        });
      }

      return throwError(() => error);
    }),
  );
};

function getApiErrorMessage(error: HttpErrorResponse): string {
  const body = error.error as ApiErrorBody | string | null;

  if (typeof body === 'string' && body.trim().length > 0) {
    return body;
  }

  if (body && typeof body === 'object') {
    return body.error ?? body.message ?? body.title ?? `Request failed with status ${error.status}.`;
  }

  return error.message || `Request failed with status ${error.status}.`;
}
