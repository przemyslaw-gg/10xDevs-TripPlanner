import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuthContext } from '../../contexts/AuthContext';

/**
 * Global navigation bar with auth-aware menu
 */
export function Navbar() {
  const { isAuthenticated, user, logout, isLoading } = useAuthContext();
  const location = useLocation();
  const navigate = useNavigate();
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleLogout = async () => {
    setIsLoggingOut(true);
    try {
      await logout();
      navigate('/login');
    } finally {
      setIsLoggingOut(false);
    }
  };

  const isActive = (path: string) => location.pathname === path;

  const navLinkClass = (path: string) =>
    `px-3 py-2 rounded-md text-sm font-medium transition-colors ${
      isActive(path)
        ? 'bg-blue-700 text-white'
        : 'text-blue-100 hover:bg-blue-600 hover:text-white'
    }`;

  const mobileNavLinkClass = (path: string) =>
    `block px-3 py-2 rounded-md text-base font-medium transition-colors ${
      isActive(path)
        ? 'bg-blue-700 text-white'
        : 'text-blue-100 hover:bg-blue-600 hover:text-white'
    }`;

  return (
    <nav className="bg-blue-800 shadow-lg">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo and brand */}
          <div className="flex items-center">
            <Link to="/" className="flex items-center gap-2">
              <svg
                className="h-8 w-8 text-white"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <span className="text-white font-bold text-xl">TripPlanner</span>
            </Link>
          </div>

          {/* Desktop navigation */}
          <div className="hidden md:flex items-center gap-4">
            {/* Nav links */}
            <div className="flex items-center gap-2">
              <Link to="/attractions" className={navLinkClass('/attractions')}>
                Atrakcje
              </Link>
              {isAuthenticated && (
                <Link to="/trips" className={navLinkClass('/trips')}>
                  Moje wycieczki
                </Link>
              )}
            </div>

            {/* Auth section */}
            <div className="flex items-center gap-3 ml-4 pl-4 border-l border-blue-600">
              {isLoading ? (
                <div className="w-20 h-8 bg-blue-700 rounded animate-pulse" />
              ) : isAuthenticated ? (
                <>
                  <span className="text-blue-200 text-sm">
                    {user?.email}
                  </span>
                  <button
                    type="button"
                    onClick={handleLogout}
                    disabled={isLoggingOut}
                    className="
                      inline-flex items-center gap-2 px-3 py-2
                      text-sm font-medium text-white
                      bg-blue-700 hover:bg-blue-600 rounded-md
                      focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-blue-800 focus:ring-white
                      disabled:opacity-50 disabled:cursor-not-allowed
                      transition-colors
                    "
                  >
                    {isLoggingOut ? (
                      <>
                        <svg
                          className="animate-spin h-4 w-4"
                          fill="none"
                          viewBox="0 0 24 24"
                        >
                          <circle
                            className="opacity-25"
                            cx="12"
                            cy="12"
                            r="10"
                            stroke="currentColor"
                            strokeWidth="4"
                          />
                          <path
                            className="opacity-75"
                            fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                          />
                        </svg>
                        Wylogowywanie...
                      </>
                    ) : (
                      <>
                        <svg
                          className="h-4 w-4"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth={2}
                            d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                          />
                        </svg>
                        Wyloguj
                      </>
                    )}
                  </button>
                </>
              ) : (
                <>
                  <Link
                    to="/login"
                    className="
                      px-3 py-2 text-sm font-medium text-blue-100
                      hover:text-white transition-colors
                    "
                  >
                    Zaloguj
                  </Link>
                  <Link
                    to="/register"
                    className="
                      px-3 py-2 text-sm font-medium text-white
                      bg-blue-600 hover:bg-blue-500 rounded-md
                      transition-colors
                    "
                  >
                    Zarejestruj
                  </Link>
                </>
              )}
            </div>
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <button
              type="button"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="
                inline-flex items-center justify-center p-2
                rounded-md text-blue-200 hover:text-white hover:bg-blue-700
                focus:outline-none focus:ring-2 focus:ring-inset focus:ring-white
              "
              aria-expanded={isMobileMenuOpen}
            >
              <span className="sr-only">Otwórz menu</span>
              {isMobileMenuOpen ? (
                <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              ) : (
                <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                </svg>
              )}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile menu */}
      {isMobileMenuOpen && (
        <div className="md:hidden border-t border-blue-700">
          <div className="px-2 pt-2 pb-3 space-y-1">
            <Link
              to="/attractions"
              className={mobileNavLinkClass('/attractions')}
              onClick={() => setIsMobileMenuOpen(false)}
            >
              Atrakcje
            </Link>
            {isAuthenticated && (
              <Link
                to="/trips"
                className={mobileNavLinkClass('/trips')}
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Moje wycieczki
              </Link>
            )}
          </div>

          {/* Mobile auth section */}
          <div className="pt-4 pb-3 border-t border-blue-700">
            {isAuthenticated ? (
              <div className="px-4 space-y-3">
                <div className="text-blue-200 text-sm">{user?.email}</div>
                <button
                  type="button"
                  onClick={() => {
                    setIsMobileMenuOpen(false);
                    handleLogout();
                  }}
                  disabled={isLoggingOut}
                  className="
                    w-full flex items-center justify-center gap-2 px-3 py-2
                    text-sm font-medium text-white
                    bg-blue-700 hover:bg-blue-600 rounded-md
                    disabled:opacity-50 disabled:cursor-not-allowed
                    transition-colors
                  "
                >
                  <svg
                    className="h-4 w-4"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                    />
                  </svg>
                  {isLoggingOut ? 'Wylogowywanie...' : 'Wyloguj'}
                </button>
              </div>
            ) : (
              <div className="px-4 space-y-2">
                <Link
                  to="/login"
                  className="block w-full text-center px-3 py-2 text-sm font-medium text-blue-100 hover:text-white transition-colors"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Zaloguj
                </Link>
                <Link
                  to="/register"
                  className="block w-full text-center px-3 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-500 rounded-md transition-colors"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Zarejestruj
                </Link>
              </div>
            )}
          </div>
        </div>
      )}
    </nav>
  );
}
