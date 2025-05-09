// client/src/components/passenger/RequestModal.tsx

import React, { useState } from 'react';
import { AvailableTripDto, TripAvailabilityDto } from '../../types';
import Button from '../ui/Button';

interface RequestModalProps {
    isOpen: boolean;
    onClose: () => void;
    trip: AvailableTripDto;
    availability: TripAvailabilityDto | undefined;
    isCheckingAvailability: boolean;
    onSubmit: (pickupAddress: string, postalCode: string) => Promise<void>;
}

const RequestModal: React.FC<RequestModalProps> = ({
    isOpen,
    onClose,
    trip,
    availability,
    isCheckingAvailability,
    onSubmit
}) => {
    const [pickupAddress, setPickupAddress] = useState<string>('');
    const [postalCode, setPostalCode] = useState<string>('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!pickupAddress.trim()) {
            setError('Please enter a pickup address');
            return;
        }
        if (!postalCode.trim()) { // Added postal code validation
            setError('Please enter a postal code');
            return;
        }
        try {
            setIsLoading(true);
            await onSubmit(pickupAddress, postalCode); // Updated to pass both values
        } catch (err) {
            console.error('Failed to submit request:', err);
            setError('Failed to submit request');
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Request Seat</h2>

                <div className="mb-4">
                    <p><strong>From:</strong> {trip.origin}</p>
                    <p><strong>To:</strong> {trip.destination}</p>
                    <p><strong>Driver:</strong> {trip.driverName}</p>
                </div>

                {availability ? (
                    <div className={`my-4 p-3 rounded-md ${availability.hasAvailableSeats
                        ? 'bg-green-100 text-green-800'
                        : 'bg-yellow-100 text-yellow-800'
                        }`}>
                        {availability.hasAvailableSeats
                            ? `${availability.availableSeats} seat${availability.availableSeats > 1 ? 's' : ''} available`
                            : (
                                <>
                                    <p className="font-medium">This trip is currently full.</p>
                                    <p className="mt-1">You can join the waiting list and will be notified if a seat becomes available.</p>
                                </>
                            )}
                    </div>
                ) : null}

                {error && (
                    <div className="my-4 p-3 bg-red-100 text-red-700 rounded-md">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block text-gray-700 mb-2">
                            Enter pickup address:
                        </label>
                        <input
                            type="text"
                            className="w-full border border-gray-300 rounded-md p-2 mb-3"
                            value={pickupAddress}
                            onChange={(e) => setPickupAddress(e.target.value)}
                            placeholder="Enter your pickup address"
                        />
                        <label className="block text-gray-700 mb-2">
                            Postal Code:
                        </label>
                        <input
                            type="text"
                            className="w-full border border-gray-300 rounded-md p-2"
                            value={postalCode}
                            onChange={(e) => setPostalCode(e.target.value)}
                            placeholder="Enter postal code"
                        />
                    </div>

                    <div className="flex justify-end space-x-3">
                        <Button
                            type="button"
                            onClick={onClose}
                            className="bg-gray-300 text-gray-800 hover:bg-gray-400"
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={isLoading}
                        >
                            {availability?.hasAvailableSeats ? 'Request Seat' : 'Join Waiting List'}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default RequestModal;