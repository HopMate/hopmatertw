// client/src/pages/driver/ManageTripPage.tsx
import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import Layout from '../../components/Layout';
import RequestCard from '../../components/trips/RequestCard';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import { tripService } from '../../services/tripService';
import { PassengerRequest, Trip } from '../../types';

const ManageTripPage: React.FC = () => {
    const { tripId } = useParams<{ tripId: string }>();

    const [trip, setTrip] = useState<Trip | null>(null);
    const [requests, setRequests] = useState<PassengerRequest[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const fetchTripRequests = async () => {
        if (!tripId) return;

        try {
            setIsLoading(true);
            setError(null);

            // Fetch trip details (assuming there's an endpoint for this)
            // If not, you can use the filtered list from parent component
            const tripsResponse = await tripService.getDriverTrips();
            const currentTrip = tripsResponse.find(t => t.id === tripId) || null;
            setTrip(currentTrip);

            // Fetch all requests for this trip
            const requestsData = await tripService.getTripRequests(tripId);
            setRequests(requestsData);
        } catch (err) {
            console.error('Error fetching trip details:', err);
            setError('Failed to load trip details. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchTripRequests();
    }, [tripId]);

    const handleCheckWaitingList = async () => {
        if (!tripId) return;

        try {
            setIsLoading(true);
            const response = await tripService.checkWaitingList(tripId);
            const movedCount = response.data.movedRequests.length;

            setSuccessMessage(`${movedCount} passengers moved from waiting list to pending status.`);
            fetchTripRequests(); // Refresh data

            // Clear success message after 5 seconds
            setTimeout(() => {
                setSuccessMessage(null);
            }, 5000);
        } catch (err) {
            console.error('Error checking waiting list:', err);
            setError('Failed to process waiting list. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    // Filter requests by status for easier presentation
    const pendingRequests = requests.filter(r => r.requestStatus.toLowerCase() === 'pending');
    const acceptedRequests = requests.filter(r => r.requestStatus.toLowerCase() === 'accepted');
    const rejectedRequests = requests.filter(r => r.requestStatus.toLowerCase() === 'rejected');
    const waitingListRequests = requests.filter(r => r.requestStatus.toLowerCase() === 'waitinglist');

    return (
        <Layout>
            <div className="p-6">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-2xl font-bold">
                        {trip ? `${trip.origin} → ${trip.destination}` : 'Trip Details'}
                    </h1>
                    <div className="flex space-x-2">
                        <Button onClick={fetchTripRequests}>Refresh</Button>
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

                {successMessage && (
                    <div className="bg-green-100 text-green-800 p-4 rounded mb-4">
                        {successMessage}
                    </div>
                )}

                {trip && (
                    <div className="bg-white shadow rounded-lg p-4 mb-6">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div>
                                <p className="text-gray-600">Departure: {new Date(trip.departureTime).toLocaleString()}</p>
                            </div>
                            <div className="flex justify-end">
                                <Badge variant={trip.availableSeats > 0 ? 'success' : 'danger'}>
                                    {trip.availableSeats} seats available
                                </Badge>
                            </div>
                        </div>
                    </div>
                )}

                {waitingListRequests.length > 0 && (
                    <div className="mb-6">
                        <div className="flex justify-between items-center mb-2">
                            <h2 className="text-lg font-semibold">Waiting List ({waitingListRequests.length})</h2>
                            <Button onClick={handleCheckWaitingList} disabled={isLoading}>
                                Check Waiting List
                            </Button>
                        </div>
                        <p className="text-sm text-gray-600 mb-4">
                            Checking the waiting list will move passengers to pending status if seats are available.
                        </p>
                    </div>
                )}

                {pendingRequests.length > 0 && (
                    <div className="mb-6">
                        <h2 className="text-lg font-semibold mb-2">Pending Requests ({pendingRequests.length})</h2>
                        <div className="space-y-4">
                            {pendingRequests.map(request => (
                                <RequestCard
                                    key={request.id}
                                    request={request}
                                    onStatusChange={fetchTripRequests}
                                />
                            ))}
                        </div>
                    </div>
                )}

                {acceptedRequests.length > 0 && (
                    <div className="mb-6">
                        <h2 className="text-lg font-semibold mb-2">Accepted Requests ({acceptedRequests.length})</h2>
                        <div className="space-y-4">
                            {acceptedRequests.map(request => (
                                <RequestCard
                                    key={request.id}
                                    request={request}
                                />
                            ))}
                        </div>
                    </div>
                )}

                {rejectedRequests.length > 0 && (
                    <div className="mb-6">
                        <h2 className="text-lg font-semibold mb-2">Rejected Requests ({rejectedRequests.length})</h2>
                        <div className="space-y-4">
                            {rejectedRequests.map(request => (
                                <RequestCard
                                    key={request.id}
                                    request={request}
                                />
                            ))}
                        </div>
                    </div>
                )}

                {waitingListRequests.length > 0 && (
                    <div className="mb-6">
                        <div className="flex justify-between items-center mb-2">
                            <h2 className="text-lg font-semibold">
                                Waiting List ({waitingListRequests.length})
                                <span className="ml-2 text-sm font-normal text-gray-500">
                                    {trip?.availableSeats || 0} seats currently available
                                </span>
                            </h2>
                            <Button
                                onClick={handleCheckWaitingList}
                                disabled={isLoading || (trip?.availableSeats || 0) <= 0}
                                variant={(trip?.availableSeats || 0) > 0 ? 'primary' : 'secondary'}
                            >
                                {isLoading ? 'Processing...' : 'Check Waiting List'}
                            </Button>
                        </div>
                        <p className="text-sm text-gray-600 mb-4">
                            Passengers will be moved from waiting list to pending status if seats are available.
                            You'll still need to approve each request.
                        </p>
                        {/* Add this section to display waiting list passengers */}
                        <div className="space-y-4">
                            {waitingListRequests.map(request => (
                                <RequestCard
                                    key={request.id}
                                    request={request}
                                />
                            ))}
                        </div>
                    </div>
                )}

                {!isLoading && requests.length === 0 && (
                    <div className="bg-gray-100 p-6 rounded text-center">
                        <p className="text-gray-600">This trip does not have any requests yet.</p>
                    </div>
                )}
            </div>
        </Layout>
    );
};

export default ManageTripPage;