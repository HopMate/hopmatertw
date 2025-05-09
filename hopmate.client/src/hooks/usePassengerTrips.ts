// client/src/hooks/usePassengerTrips.ts
import { useState, useEffect, useCallback } from 'react';
import {
    AvailableTripDto,
    PassengerTripDto,
    TripAvailabilityDto
} from '../types';
import { passengerService } from '../services/passengerService';

export const usePassengerTrips = () => {
    const [availableTrips, setAvailableTrips] = useState<AvailableTripDto[]>([]);
    const [myRequests, setMyRequests] = useState<PassengerTripDto[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchAvailableTrips = useCallback(async () => {
        try {
            setIsLoading(true);
            setError(null);
            const data = await passengerService.getAvailableTrips();
            setAvailableTrips(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Error fetching available trips:', err);
            setAvailableTrips([]);
            setError('Failed to load available trips. Please try again.');
        } finally {
            setIsLoading(false);
        }
    }, []);

    const fetchMyRequests = useCallback(async () => {
        try {
            setIsLoading(true);
            setError(null);
            const data = await passengerService.getMyRequests();
            setMyRequests(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Error fetching my requests:', err);
            setMyRequests([]);
            setError('Failed to load your trip requests. Please try again.');
        } finally {
            setIsLoading(false);
        }
    }, []);

    const checkTripAvailability = useCallback(async (tripId: string): Promise<TripAvailabilityDto> => {
        try {
            return await passengerService.checkTripAvailability(tripId);
        } catch (err) {
            console.error(`Error checking availability for trip ${tripId}:`, err);
            throw err;
        }
    }, []);

    // client/src/hooks/usePassengerTrips.ts
    const createRequest = useCallback(async (tripId: string, pickupAddress: string, postalCode: string) => {
        try {
            setIsLoading(true);
            const result = await passengerService.createParticipationRequest({
                tripId,
                locationId: pickupAddress, // Still using locationId field for address
                postalCode  // Add postal code
            });
            await fetchMyRequests();
            return result;
        } catch (err) {
            console.error('Error creating participation request:', err);
            throw err;
        } finally {
            setIsLoading(false);
        }
    }, [fetchMyRequests]);

    // Similarly update joinWaitingList

    const joinWaitingList = useCallback(async (tripId: string, pickupAddress: string, postalCode: string) => {
        try {
            setIsLoading(true);
            const result = await passengerService.joinWaitingList({
                tripId,
                locationId: pickupAddress, // Using locationId field to pass address
                postalCode
            });
            await fetchMyRequests(); // Refresh requests after joining waiting list
            return result;
        } catch (err) {
            console.error('Error joining waiting list:', err);
            throw err;
        } finally {
            setIsLoading(false);
        }
    }, [fetchMyRequests]);

    const refreshData = useCallback(() => {
        fetchAvailableTrips();
        fetchMyRequests();
    }, [fetchAvailableTrips, fetchMyRequests]);

    useEffect(() => {
        refreshData();
    }, [refreshData]);

    return {
        availableTrips,
        myRequests,
        isLoading,
        error,
        refreshData,
        checkTripAvailability,
        createRequest,
        joinWaitingList
    };
};