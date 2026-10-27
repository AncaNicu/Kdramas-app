import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (request, next) => 
{
    //read the token from sessionStorage
    const token = sessionStorage.getItem('token');

    //check if the request URL belongs to our app
    const isApiRequest = request.url.startsWith(environment.apiUrl);

    //it should skip login and register requests
    const isAuthRequest =
        request.url.includes('/api/auth/login') ||
        request.url.includes('/api/auth/register');

    //if there is no token
    //or the request URL belongs to the BE
    //or is an auth request
    //send the request without the token
    if (!token || !isApiRequest || isAuthRequest) 
    {
        return next(request);
    }

    //clone the request with the token added
    const authRequest = request.clone
    ({
        setHeaders: 
        {
            Authorization: `Bearer ${token}`
        }
    });

    //send the request with the token attached
    return next(authRequest);
};
