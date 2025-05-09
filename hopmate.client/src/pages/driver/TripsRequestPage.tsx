// client/src/pages/driver/TripRequestsPage.tsx
import React from 'react';
import { Link } from 'react-router-dom';
import Layout from '../../components/Layout';
import RequestCard from '../../components/trips/RequestCard';
import Button from '../../components/ui/Button';
import { useDriverTrips } from '../../hooks/useDriverTrips';

const TripRequestsPage: React.FC = () => {
    const { pendingRequests, isLoading, error, refreshData } = useDriverTrips();

    return (
        <Layout>
            <div className="p-6">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-2xl font-bold">Pending Requests</h1>
                    <div className="flex space-x-2">
                        <Button onClick={refreshData}>Refresh</Button>
                        <Link to="/driver/trips">
                            <Button variant="secondary">Back to My Trips</Button>
                        </Link>
                    </div>
                </div>

                {isLoading && <p className="text-center py-4">Loading...</p>}

                {error && (
                    <div className="bg-red-100 text-red-800 p-4 rounded mb-4">
                        {error}
                    </div>
                )}

                {!isLoading && pendingRequests.length === 0 && (
                    <div className="bg-gray-100 p-6 rounded text-center">
                        <p className="text-gray-600">You don't have any pending requests.</p>
                    </div>
                )}

                <div className="space-y-4">
                    {pendingRequests.map(request => (
                        <RequestCard
                            key={request.id}
                            request={request}
                            onStatusChange={refreshData}
                        />
                    ))}
                </div>
            </div>
        </Layout>
    );
};

export default TripRequestsPage;