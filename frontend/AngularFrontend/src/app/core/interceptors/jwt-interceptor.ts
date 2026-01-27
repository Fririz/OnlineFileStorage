import { HttpInterceptorFn } from '@angular/common/http';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const cloned = req.clone({
      withCredentials: true
    });

  return next(cloned);
};
