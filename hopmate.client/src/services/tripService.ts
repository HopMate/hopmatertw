// client/src/services/tripService.ts

import { axiosInstance } from '../axiosConfig';
import { Trip, PassengerRequest, AcceptRequestPayload, RejectRequestPayload } from '../types';

// Define backend response type to match the actual data structure
interface RequestDataFromBackend {
    Id?: string;
    PassengerId?: string;
    PassengerName?: string;
    TripId?: string;
    LocationId?: string;
    PickupLocation?: string;
    RequestStatusId?: number;
    RequestStatus?: string;
    RequestDate?: string;
    Reason?: string | null;
}

export const tripService = {
    // Get all trips for the current driver
    getDriverTrips: async (): Promise<Trip[]> => {
        try {
            console.log('Fetching driver trips...');
            const response = await axiosInstance.get('/DriverTrip/trips');
            console.log('Raw API response:', response.data);

            // Handle various possible response formats
            let tripsData;
            if (Array.isArray(response.data)) {
                tripsData = response.data;
            } else if (response.data.$values) {
                tripsData = response.data.$values;
            } else {
                tripsData = [response.data]; // Handle a single object response
            }

            // Log data after parsing
            console.log('Parsed trips data:', tripsData);

            // Ensure we're working with an array
            if (!Array.isArray(tripsData)) {
                console.error('Failed to parse trips data into array:', tripsData);
                return [];
            }

            // Map the backend data to our frontend model
            // NOTE: Modified to use proper case mapping from backend fields
            return tripsData.map(trip => ({
                id: trip.Id || '',
                origin: trip.Origin || 'Unknown origin',
                destination: trip.Destination || 'Unknown destination',
                departureTime: trip.DepartureTime || new Date().toISOString(),
                availableSeats: typeof trip.AvailableSeats === 'number' ? trip.AvailableSeats : 0,
                status: trip.Status || 'Unknown',
                requestCount: typeof trip.RequestCount === 'number' ? trip.RequestCount : 0
            }));
        } catch (error) {
            console.error('Error in getDriverTrips:', error);
            // Return empty array on error to avoid null/undefined issues
            return [];
        }
    },

    // Get all pending requests for all trips
    getPendingRequests: async (): Promise<PassengerRequest[]> => {
        try {
            console.log('Fetching pending requests...');
            const response = await axiosInstance.get('/DriverTrip/pendingrequests');
            console.log('Pending requests response:', response.data);

            // Handle various possible response formats
            let requestsData;
            if (Array.isArray(response.data)) {
                requestsData = response.data;
            } else if (response.data.$values) {
                requestsData = response.data.$values;
            } else if (response.data && typeof response.data === 'object') {
                requestsData = [response.data]; // Handle single object
            } else {
                requestsData = [];
            }

            // NOTE: Modified to match field casing from backend
            return requestsData.map((req: RequestDataFromBackend) => ({
                id: req.Id || '',
                passengerId: req.PassengerId || '',
                passengerName: req.PassengerName || 'Unknown passenger',
                tripId: req.TripId || '',
                locationId: req.LocationId || '',
                pickupLocation: req.PickupLocation || 'Unknown location',
                requestStatusId: req.RequestStatusId || 0,
                requestStatus: req.RequestStatus || 'Unknown',
                requestDate: req.RequestDate || new Date().toISOString(),
                reason: req.Reason || null
            }));
        } catch (error) {
            console.error('Error in getPendingRequests:', error);
            return [];
        }
    },

    // Get all requests for a specific trip
    getTripRequests: async (tripId: string): Promise<PassengerRequest[]> => {
        try {
            const response = await axiosInstance.get(`/DriverTrip/trips/${tripId}/requests`);

            // Ensure we return an array even if the API doesn't
            let requestsData;
            if (Array.isArray(response.data)) {
                requestsData = response.data;
            } else if (response.data.$values) {
                requestsData = response.data.$values;
            } else {
                console.error('API returned non-array for trip requests:', response.data);
                return [];
            }

            // NOTE: Modified to match field casing from backend
            return requestsData.map((req: RequestDataFromBackend) => ({
                id: req.Id || '',
                passengerId: req.PassengerId || '',
                passengerName: req.PassengerName || 'Unknown passenger',
                tripId: req.TripId || '',
                locationId: req.LocationId || '',
                pickupLocation: req.PickupLocation || 'Unknown location',
                requestStatusId: req.RequestStatusId || 0,
                requestStatus: req.RequestStatus || 'Unknown',
                requestDate: req.RequestDate || new Date().toISOString(),
                reason: req.Reason || null
            }));
        } catch (error) {
            console.error(`Error getting requests for trip ${tripId}:`, error);
            throw error;
        }
    },

    // Accept a request
    acceptRequest: async (payload: AcceptRequestPayload) => {
        try {
            return await axiosInstance.post('/DriverTrip/accept', payload);
        } catch (error) {
            console.error('Error accepting request:', error);
            throw error;
        }
    },

    // Reject a request
    rejectRequest: async (payload: RejectRequestPayload) => {
        try {
            return await axiosInstance.post('/DriverTrip/reject', payload);
        } catch (error) {
            console.error('Error rejecting request:', error);
            throw error;
        }
    },

    // Check waiting list for a trip
    checkWaitingList: async (tripId: string) => {
        try {
            return await axiosInstance.post(`/DriverTrip/trips/${tripId}/checkwaitinglist`);
        } catch (error) {
            console.error(`Error checking waiting list for trip ${tripId}:`, error);
            throw error;
        }
    }
};
