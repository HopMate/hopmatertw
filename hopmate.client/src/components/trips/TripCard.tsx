// client/src/components/trips/TripCard.tsx
import React from 'react';
import { Link } from 'react-router-dom';
import { Trip } from '../../types';
import Badge from '../ui/Badge';
import Button from '../ui/Button';

interface TripCardProps {
    trip: Trip;
    pendingRequestsCount?: number;
}

const TripCard: React.FC<TripCardProps> = ({ trip, pendingRequestsCount = 0 }) => {
    // Better date handling with fallback
    const formatDate = (dateString: string | undefined) => {
        if (!dateString) return 'Date not available';

        try {
            const date = new Date(dateString);
            // Check if date is valid
            if (isNaN(date.getTime())) {
                return 'Invalid date';
            }
            return date.toLocaleString();
        } catch (error) {
            console.error('Error formatting date:', error);
            return 'Date error';
        }
    };

    // Add debug info for trip data
    console.log('Trip in TripCard:', trip);

    return (
        <div className="bg-white shadow rounded-lg p-4 mb-4">
            <div className="flex justify-between items-start mb-2">
                <h3 className="text-lg font-medium">
                    {trip.origin || 'Unknown origin'} → {trip.destination || 'Unknown destination'}
                </h3>
                <Badge variant={(trip.availableSeats > 0) ? 'success' : 'danger'}>
                    {typeof trip.availableSeats === 'number' ? trip.availableSeats : 0} seats available
                </Badge>
            </div>

            <div className="text-sm text-gray-600 mb-4">
                <p>Departure: {formatDate(trip.departureTime)}</p>
                <p>Status: {trip.status || 'Unknown'}</p>
            </div>

            <div className="flex justify-between items-center">
                {pendingRequestsCount > 0 && (
                    <Badge variant="warning" className="mr-2">
                        {pendingRequestsCount} pending requests
                    </Badge>
                )}
                <div className="space-x-2">
                    <Link to={`/driver/trips/${trip.id}/requests`}>
                        <Button variant="primary">Manage Requests</Button>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default TripCard;