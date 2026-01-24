import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AttractionsPage } from './pages/AttractionsPage';

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Attractions routes */}
        <Route path="/attractions" element={<AttractionsPage />} />

        {/* Redirect root to attractions */}
        <Route path="/" element={<Navigate to="/attractions" replace />} />

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
    </BrowserRouter>
  );
}

export default App;
