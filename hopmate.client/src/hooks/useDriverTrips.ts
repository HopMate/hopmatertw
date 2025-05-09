// client/src/hooks/useDriverTrips.ts

import { useState, useEffect, useCallback } from 'react';
import { Trip, PassengerRequest } from '../types';
import { tripService } from '../services/tripService';

export const useDriverTrips = () => {
    const [trips, setTrips] = useState<Trip[]>([]);  // Initialize as empty array
    const [pendingRequests, setPendingRequests] = useState<PassengerRequest[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchDriverTrips = useCallback(async () => {
        try {
            setIsLoading(true);
            setError(null);
            const data = await tripService.getDriverTrips();

            // Ensure data is an array before setting it
            if (Array.isArray(data)) {
                setTrips(data);
            } else {
                console.error('API returned non-array data for trips:', data);
                setTrips([]);  // Set to empty array if data is not an array
                setError('Invalid data format received from server');
            }
        } catch (err) {
            console.error('Error fetching driver trips:', err);
            setTrips([]);  // Ensure trips is an array in case of error
            setError('Failed to load trips. Please try again.');
        } finally {
            setIsLoading(false);
        }
    }, []);

    const fetchPendingRequests = useCallback(async () => {
        try {
            setIsLoading(true);
            setError(null);
            const data = await tripService.getPendingRequests();

            // Ensure data is an array before setting it
            if (Array.isArray(data)) {
                setPendingRequests(data);
            } else {
                console.error('API returned non-array data for pending requests:', data);
                setPendingRequests([]);  // Set to empty array if data is not an array
                setError('Invalid data format received from server');
            }
        } catch (err) {
            console.error('Error fetching pending requests:', err);
            setPendingRequests([]);  // Ensure pendingRequests is an array in case of error
            setError('Failed to load pending requests. Please try again.');
        } finally {
            setIsLoading(false);
        }
    }, []);

    const refreshData = useCallback(() => {
        fetchDriverTrips();
        fetchPendingRequests();
    }, [fetchDriverTrips, fetchPendingRequests]);

    useEffect(() => {
        refreshData();
    }, [refreshData]);

    const getPendingRequestsForTrip = useCallback((tripId: string) => {
        return pendingRequests.filter(request => request.tripId === tripId);
    }, [pendingRequests]);

    return {
        trips,
        pendingRequests,
        isLoading,
        error,
        refreshData,
        getPendingRequestsForTrip
    };
};