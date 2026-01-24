import { useNavigate, Link } from 'react-router-dom';
import { LoginForm } from '../components/auth';

export function LoginPage() {
  const navigate = useNavigate();

  const handleSuccess = () => {
    navigate('/', { replace: true });
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        {/* Logo / App Name */}
        <h1 className="text-center text-3xl font-bold text-gray-900">
          TripPlanner
        </h1>
        <h2 className="mt-6 text-center text-xl font-semibold text-gray-700">
          Zaloguj się do swojego konta
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <LoginForm onSuccess={handleSuccess} />

          {/* Register Link */}
          <div className="mt-6">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-300" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-white text-gray-500">
                  Nie masz jeszcze konta?
                </span>
              </div>
            </div>

            <div className="mt-6">
              <Link
                to="/register"
                className="
                  w-full flex justify-center py-2.5 px-4
                  border border-gray-300 rounded-md shadow-sm
                  text-sm font-medium text-gray-700
                  bg-white hover:bg-gray-50
                  focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
                  transition-colors duration-150
                "
              >
                Zarejestruj się
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
