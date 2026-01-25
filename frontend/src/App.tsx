import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { PublicRoute } from './components/common/PublicRoute';
import { ProtectedRoute } from './components/common/ProtectedRoute';
import { Navbar } from './components/common/Navbar';
import { AttractionsPage } from './pages/AttractionsPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { TripsPage } from './pages/TripsPage';
import { CreateTripPage } from './pages/CreateTripPage';
import { TripDetailsPage } from './pages/TripDetailsPage';
import { CreateAttractionPage } from './pages/CreateAttractionPage';

export function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Navbar />
        <Routes>
        {/* Auth routes */}
        <Route
          path="/login"
          element={
            <PublicRoute>
              <LoginPage />
            </PublicRoute>
          }
        />
        <Route
          path="/register"
          element={
            <PublicRoute>
              <RegisterPage />
            </PublicRoute>
          }
        />

        {/* Trips routes (protected) */}
        <Route
          path="/trips"
          element={
            <ProtectedRoute>
              <TripsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/trips/new"
          element={
            <ProtectedRoute>
              <CreateTripPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/trips/:id"
          element={
            <ProtectedRoute>
              <TripDetailsPage />
            </ProtectedRoute>
          }
        />

        {/* Attractions routes */}
        <Route path="/attractions" element={<AttractionsPage />} />
        <Route
          path="/attractions/new"
          element={
            <ProtectedRoute>
              <CreateAttractionPage />
            </ProtectedRoute>
          }
        />

        {/* Redirect root to trips (main view for logged in users) */}
        <Route path="/" element={<Navigate to="/trips" replace />} />

        {/* 404 fallback */}
        <Route
          path="*"
          element={
            <div className="min-h-screen bg-gray-50 flex items-center justify-center">
              <div className="text-center">
                <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
                <p className="text-gray-600 mb-4">Strona nie została znaleziona</p>
                <a
                  href="/attractions"
                  className="text-blue-600 hover:text-blue-800 underline"
                >
                  Wróć do listy atrakcji
                </a>
              </div>
            </div>
          }
        />
      </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
