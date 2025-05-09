import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import PrivateRoute from './components/PrivateRoute';
import CancelTripPage from './pages/TripCancellation/CancelTripPage';

import DriverTripsPage from './pages/driver/DriverTripsPage';
import ManageTripPage from './pages/driver/ManageTripPage';
import TripRequestsPage from './pages/driver/TripsRequestPage';

import MyTripsPage from './pages/passenger/MyTripsPage';
import AvailableTripsPage from './pages/passenger/AvailableTripsPage';

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route
                    path="/dashboard"
                    element={
                        <PrivateRoute>
                            <Dashboard />
                        </PrivateRoute>
                    }
                />

                <Route
                    path="/driver/trips/:tripId/requests"
                    element={
                        <PrivateRoute>
                            <ManageTripPage />
                        </PrivateRoute>
                    }
                />

                <Route
                    path="/driver/trips"
                    element={
                        <PrivateRoute>
                            <DriverTripsPage />
                        </PrivateRoute>
                    }
                />

                <Route
                    path="/driver/requests"
                    element={
                        <PrivateRoute>
                            <TripRequestsPage />
                        </PrivateRoute>
                    }
                />

                <Route
                    path="/passenger/my-trips"
                    element={
                        <PrivateRoute>
                            <MyTripsPage />
                        </PrivateRoute>
                    }
                />

                <Route
                    path="/passenger/available-trips"
                    element={
                        <PrivateRoute>
                            <AvailableTripsPage />
                        </PrivateRoute>
                    }
                />

                <Route path="/trip/cancel/:id" element={<CancelTripPage />} />
            </Routes>
        </Router>
    );
};

export default App;