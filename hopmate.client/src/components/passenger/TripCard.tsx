// client/src/components/passenger/TripCard.tsx
import React from 'react';
import { format } from 'date-fns';
import { AvailableTripDto, TripAvailabilityDto } from '../../types';
import Button from '../ui/Button';
interface TripCardProps {
    trip: AvailableTripDto;
    onSelect: (trip: AvailableTripDto) => void;
    isRequested: boolean;
    availability?: TripAvailabilityDto; // Make it optional
}
const TripCard: React.FC<TripCardProps> = ({ trip, onSelect, isRequested, availability }) => {
    const formatDepartureTime = (departureTime: string) => {
        try {
            return format(new Date(departureTime), 'MMM dd, yyyy - HH:mm');
        } catch (error) {
            console.error('Error formatting date:', error);
            return 'Invalid date';
        }
    };
    // Get available seats - use availability if it exists, otherwise fall back to trip.availableSeats
    const availableSeats = availability?.availableSeats ?? trip.availableSeats;
    return (
        <div className="bg-white rounded-lg shadow-md p-6 flex flex-col">
            <div className="mb-4">
                <h3 className="text-xl font-semibold mb-2 text-gray-800">{trip.origin} → {trip.destination}</h3>
                <p className="text-gray-600">
                    <span className="font-medium">Departure:</span> {formatDepartureTime(trip.departureTime)}
                </p>
            </div>
            <div className="flex items-center mb-4">
                <div className="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm font-medium mr-2">
                    {availableSeats} seats available
                </div>
                <div className="text-gray-600 text-sm">Driver: {trip.driverName}</div>
            </div>
            <div className="mt-auto">
                <Button
                    onClick={() => onSelect(trip)}
                    disabled={isRequested}
                    className={isRequested ? "bg-gray-400" : ""}
                >
                    {isRequested ? 'Request Pending' : 'Request Seat'}
                </Button>
            </div>
        </div>
    );
};
export default TripCard;