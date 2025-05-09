// client/src/pages/passenger/AvailableTripsPage.tsx
import React, { useState } from 'react';
import { usePassengerTrips } from '../../hooks/usePassengerTrips';
import TripCard from '../../components/passenger/TripCard';
import RequestModal from '../../components/passenger/RequestModal';
import Button from '../../components/ui/Button';
import Layout from '../../components/Layout';

import { AvailableTripDto, TripAvailabilityDto } from '../../types/index';

const AvailableTripsPage: React.FC = () => {
    const {
        availableTrips,
        myRequests,
        isLoading,
        error,
        refreshData,
        checkTripAvailability,
        createRequest,
        joinWaitingList
    } = usePassengerTrips();

    const [selectedTrip, setSelectedTrip] = useState<AvailableTripDto | null>(null);
    const [showModal, setShowModal] = useState(false);
    const [availability, setAvailability] = useState<TripAvailabilityDto | null>(null);
    const [isCheckingAvailability, setIsCheckingAvailability] = useState(false);

    const handleTripSelect = async (trip: AvailableTripDto) => {
        setSelectedTrip(trip);
        setIsCheckingAvailability(true);
        try {
            const availability = await checkTripAvailability(trip.id);
            setAvailability(availability);
        } catch (err) {
            console.error('Error checking availability:', err);
        } finally {
            setIsCheckingAvailability(false);
        }
        setShowModal(true);
    };

    // client/src/pages/passenger/AvailableTripsPage.tsx
    // Update the handleRequestSubmit function
    const handleRequestSubmit = async (pickupAddress: string, postalCode: string) => {
        if (!selectedTrip) return;

        try {
            if (availability?.hasAvailableSeats) {
                await createRequest(selectedTrip.id, pickupAddress, postalCode);
            } else {
                await joinWaitingList(selectedTrip.id, pickupAddress, postalCode);
            }
            setShowModal(false);
            refreshData();
        } catch (err) {
            console.error('Error submitting request:', err);
        }
    };

    return (
        <Layout>
            <div className="container mx-auto px-4 py-8">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-2xl font-bold">Available Trips</h1>
                    <Button onClick={refreshData} disabled={isLoading}>
                        Refresh
                    </Button>
                </div>

                {isLoading && !availableTrips.length && (
                    <div className="flex justify-center my-12">
                        Loading
                    </div>
                )}

                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-6">
                        {error}
                    </div>
                )}

                {!isLoading && availableTrips.length === 0 && !error && (
                    <div className="text-center py-12">
                        <p className="text-gray-500 text-lg">No available trips found</p>
                    </div>
                )}

                <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {availableTrips.map(trip => (
                        <TripCard
                            key={trip.id}
                            trip={trip}
                            onSelect={handleTripSelect}
                            isRequested={myRequests.some(r => r.tripId === trip.id)}
                            availability={selectedTrip?.id === trip.id ? availability : undefined}
                        />
                    ))}
                </div>

                {selectedTrip && (
                    <RequestModal
                        isOpen={showModal}
                        onClose={() => setShowModal(false)}
                        trip={selectedTrip}
                        availability={availability}
                        isCheckingAvailability={isCheckingAvailability}
                        onSubmit={handleRequestSubmit}
                    />
                )}
            </div>
        </Layout>
    );
};

export default AvailableTripsPage;