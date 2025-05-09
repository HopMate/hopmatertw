// client/src/pages/driver/DriverTripsPage.tsx
import React from 'react';
import { Link } from 'react-router-dom';
import Layout from '../../components/Layout';
import TripCard from '../../components/trips/TripCard';
import Button from '../../components/ui/Button';
import { useDriverTrips } from '../../hooks/useDriverTrips';

const DriverTripsPage: React.FC = () => {
    const { trips, pendingRequests, isLoading, error, refreshData, getPendingRequestsForTrip } = useDriverTrips();

    return (
        <Layout>
            <div className="p-6">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-2xl font-bold">My Trips</h1>    
                    <div className="flex space-x-2">
                        <Button onClick={refreshData}>Refresh</Button>
                    </div>
                </div>

                {isLoading && <p className="text-center py-4">Loading...</p>}

                {error && (
                    <div className="bg-red-100 text-red-800 p-4 rounded mb-4">
                        {error}
                    </div>
                )}

                {!isLoading && trips.length === 0 && (
                    <div className="bg-gray-100 p-6 rounded text-center">
                        <p className="text-gray-600">You don't have any trips yet.</p>
                    </div>
                )}

                <div className="space-y-4">
                    {trips.map(trip => (
                        <TripCard
                            key={trip.id}
                            trip={trip} 
                            pendingRequestsCount={getPendingRequestsForTrip(trip.id).length}
                        />
                    ))}
                </div>

                {pendingRequests.length > 0 && (
                    <div className="mt-8">
                        <h2 className="text-xl font-semibold mb-4">All Pending Requests</h2>
                        <Link to="/driver/requests">
                            <Button>View All Pending Requests ({pendingRequests.length})</Button>
                        </Link>
                    </div>
                )}
            </div>
        </Layout>
    );
};

export default DriverTripsPage;