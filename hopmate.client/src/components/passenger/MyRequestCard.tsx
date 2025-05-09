// client/src/components/passenger/MyRequestCard.tsx

import React from 'react';
import { PassengerTripDto } from '../../types';
import Badge from '../ui/Badge';

interface MyRequestCardProps {
    request: PassengerTripDto;
}

const MyRequestCard: React.FC<MyRequestCardProps> = ({ request }) => {
    const getStatusBadgeVariant = () => {
        switch (request.requestStatus?.toLowerCase()) {
            case 'pending':
                return 'warning';
            case 'accepted':
                return 'success';
            case 'rejected':
                return 'danger';
            case 'waitinglist':
                return 'secondary';
            default:
                return 'primary';
        }
    };

    const getStatusDescription = () => {
        switch (request.requestStatus?.toLowerCase()) {
            case 'waitinglist':
                return 'You are on the waiting list. You will be notified if a seat becomes available.';
            case 'pending':
                return 'Your request is pending approval by the driver.';
            case 'accepted':
                return 'Your request has been accepted!';
            case 'rejected':
                return 'Your request has been rejected.';
            default:
                return '';
        }
    };

    const formatDate = (dateString: string) => {
        try {
            const date = new Date(dateString);
            if (isNaN(date.getTime())) return 'Invalid date';
            return date.toLocaleString();
        } catch (error) {
            console.error('Error formatting date:', error);
            return 'Date error';
        }
    };

    return (
        <div className="bg-white shadow rounded-lg p-4 mb-4">
            <div className="flex justify-between items-start mb-2">
                <h3 className="text-lg font-medium">
                    {request.trip?.origin || 'Unknown'} → {request.trip?.destination || 'Unknown'}
                </h3>
                <Badge variant={getStatusBadgeVariant()}>
                    {request.requestStatus || 'Unknown status'}
                </Badge>
            </div>

            <div className="text-sm text-gray-600 mb-2">
                <p>Departure: {formatDate(request.trip?.departureTime || '')}</p>
                <p>Pickup Location: {request.pickupLocation || 'Not specified'}</p>
                <p>Request Date: {formatDate(request.requestDate)}</p>
                {request.trip?.driverName && <p>Driver: {request.trip.driverName}</p>}
            </div>

            {request.requestStatus?.toLowerCase() === 'rejected' && request.reason && (
                <div className="mt-2 p-2 bg-red-50 border border-red-200 rounded">
                    <p className="text-sm text-red-700">
                        <span className="font-semibold">Rejection reason:</span> {request.reason}
                    </p>
                </div>
            )}

            <div className="mt-2 text-sm text-gray-600">
                <p>{getStatusDescription()}</p>
                {request.requestStatus?.toLowerCase() === 'waitinglist' && (
                    <p className="text-blue-600 mt-1">
                        Your position in queue: {request.queuePosition || 'N/A'}
                    </p>
                )}
            </div>
        </div>
    );
};

export default MyRequestCard;