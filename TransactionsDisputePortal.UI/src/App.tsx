import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import ProtectedRoute from './components/ProtectedRoute'
import Dashboard from './pages/Dashboard'
import Transactions from './pages/Transactions'
import Disputes from './pages/Disputes'
import Login from './pages/Login'
import { useAuth } from './contexts/AuthContext'

function App() {
  const { isAuthenticated } = useAuth()

  return (
    <Routes>
      <Route
        path="/login"
        element={isAuthenticated ? <Navigate to="/" replace /> : <Login />}
      />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout>
              <Dashboard />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/transactions"
        element={
          <ProtectedRoute>
            <Layout>
              <Transactions />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/disputes"
        element={
          <ProtectedRoute>
            <Layout>
              <Disputes />
            </Layout>
          </ProtectedRoute>
        }
      />
    </Routes>
  )
}

export default App
