// client/src/services/passengerService.ts

import { axiosInstance } from '../axiosConfig';
import {
    AvailableTripDto,
    TripAvailabilityDto,
    CreateParticipationRequestDto,
    JoinWaitingListDto,
    PassengerTripDto,
    LocationDto
} from '../types';

export const passengerService = {
    // Get all available trips for passengers
    getAvailableTrips: async (): Promise<AvailableTripDto[]> => {
        try {
            const response = await axiosInstance.get('/PassengerTrip/available', {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            console.log('Available trips response:', response);

            // Handle various possible response formats
            let tripsData;
            if (Array.isArray(response.data)) {
                tripsData = response.data;
            } else if (response.data?.$values) {
                tripsData = response.data.$values;
            } else {
                tripsData = [response.data];
            }

            return tripsData.map((trip: any) => ({
                id: trip.id || trip.Id || '',
                origin: trip.origin || trip.Origin || 'Unknown origin',
                destination: trip.destination || trip.Destination || 'Unknown destination',
                departureTime: trip.departureTime || trip.DepartureTime || new Date().toISOString(),
                availableSeats: typeof trip.availableSeats === 'number' ? trip.availableSeats :
                    typeof trip.AvailableSeats === 'number' ? trip.AvailableSeats : 0,
                status: trip.status || trip.Status || 'Unknown',
                driverName: trip.driverName || trip.DriverName || 'Unknown driver',
                price: trip.price || trip.Price || 0
            }));
        } catch (error) {
            console.error('Detailed error fetching available trips:', {
                error: error,
                response: error.response,
                config: error.config
            });
            return [];
        }
    },

    // Check if a trip has available seats
    checkTripAvailability: async (tripId: string): Promise<TripAvailabilityDto> => {
        try {
            const response = await axiosInstance.get(`/PassengerTrip/trips/${tripId}/availability`);
            return {
                tripId: response.data.TripId || tripId,
                hasAvailableSeats: response.data.HasAvailableSeats || false,
                availableSeats: response.data.AvailableSeats || 0,
                totalSeats: response.data.TotalSeats || 0
            };
        } catch (error) {
            console.error(`Error checking availability for trip ${tripId}:`, error);
            // Return default values on error
            return {
                tripId,
                hasAvailableSeats: false,
                availableSeats: 0,
                totalSeats: 0
            };
        }
    },

    // Create a participation request
    createParticipationRequest: async (request: CreateParticipationRequestDto): Promise<PassengerTripDto> => {
        try {
            const response = await axiosInstance.post('/PassengerTrip/request', {
                tripId: request.tripId,
                pickupAddress: request.locationId,  // Using locationId as the pickup address string
                postalCode: request.postalCode
            }, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });
            const data = response.data;

            return {
                id: data.Id || '',
                passengerId: data.PassengerId || '',
                passengerName: data.PassengerName,
                tripId: data.TripId || '',
                locationId: data.LocationId || '',
                pickupLocation: data.PickupLocation || request.locationId, // Use the address we sent if not returned
                requestStatusId: data.RequestStatusId || 1, // Default to pending
                requestStatus: data.RequestStatus || 'Pending',
                requestDate: data.RequestDate || new Date().toISOString(),
                reason: data.Reason
            };
        } catch (error) {
            console.error('Detailed error creating participation request:', {
                error: error,
                response: error.response?.data,
                config: error.config
            });
            throw error;
        }
    },

    // Join a waiting list
    joinWaitingList: async (request: JoinWaitingListDto): Promise<PassengerTripDto> => {
        try {
            const response = await axiosInstance.post('/PassengerTrip/waitinglist', {
                TripId: request.tripId,
                PickupAddress: request.locationId, // Using locationId as the pickup address string
                postalCode: request.postalCode
            });
            const data = response.data;

            return {
                id: data.Id || '',
                passengerId: data.PassengerId || '',
                passengerName: data.PassengerName,
                tripId: data.TripId || '',
                locationId: data.LocationId || '',
                pickupLocation: data.PickupLocation || request.locationId, // Use the address we sent if not returned
                requestStatusId: data.RequestStatusId || 4, // Default to waiting list
                requestStatus: data.RequestStatus || 'WaitingList',
                requestDate: data.RequestDate || new Date().toISOString(),
                reason: data.Reason
            };
        } catch (error) {
            console.error('Error joining waiting list:', error);
            throw error;
        }
    },

    // Get all requests made by the current passenger
    getMyRequests: async (): Promise<PassengerTripDto[]> => {
        try {
            const response = await axiosInstance.get('/PassengerTrip/myrequests');

            // Handle various possible response formats
            let requestsData;
            if (Array.isArray(response.data)) {
                requestsData = response.data;
            } else if (response.data.$values) {
                requestsData = response.data.$values;
            } else {
                requestsData = [response.data]; // Handle a single object response
            }

            // Map the backend data to our frontend model
            return requestsData.map((req: any) => ({
                id: req.Id || '',
                passengerId: req.PassengerId || '',
                passengerName: req.PassengerName,
                tripId: req.TripId || '',
                locationId: req.LocationId || '',
                pickupLocation: req.PickupLocation || req.LocationName,
                requestStatusId: req.RequestStatusId || 0,
                requestStatus: req.RequestStatus || 'Unknown',
                requestDate: req.RequestDate || new Date().toISOString(),
                reason: req.Reason,
                previousStatus: req.PreviousStatus,
                queuePosition: req.QueuePosition,
                trip: req.Trip ? {
                    origin: req.Trip.Origin || 'Unknown',
                    destination: req.Trip.Destination || 'Unknown',
                    departureTime: req.Trip.DepartureTime || new Date().toISOString(),
                    driverName: req.Trip.DriverName,
                    availableSeats: req.Trip.AvailableSeats || 0
                } : undefined
            }));
        } catch (error) {
            console.error('Error fetching passenger requests:', error);
            return [];
        }
    },

    // Get a specific request by ID
    getRequestById: async (requestId: string): Promise<PassengerTripDto> => {
        try {
            const response = await axiosInstance.get(`/PassengerTrip/${requestId}`);
            const data = response.data;

            return {
                id: data.Id || '',
                passengerId: data.PassengerId || '',
                passengerName: data.PassengerName,
                tripId: data.TripId || '',
                locationId: data.LocationId || '',
                pickupLocation: data.PickupLocation || data.LocationName,
                requestStatusId: data.RequestStatusId || 0,
                requestStatus: data.RequestStatus || 'Unknown',
                requestDate: data.RequestDate || new Date().toISOString(),
                reason: data.Reason,
                trip: data.Trip ? {
                    origin: data.Trip.Origin || 'Unknown',
                    destination: data.Trip.Destination || 'Unknown',
                    departureTime: data.Trip.DepartureTime || new Date().toISOString(),
                    driverName: data.Trip.DriverName
                } : undefined
            };
        } catch (error) {
            console.error(`Error fetching request with ID ${requestId}:`, error);
            throw error;
        }
    },

    // Get locations for a trip - we'll keep this method for backward compatibility
    getTripLocations: async (tripId: string): Promise<LocationDto[]> => {
        try {
            const response = await axiosInstance.get(`/PassengerTrip/${tripId}/locations`);
            return response.data.map((loc: any) => ({
                id: loc.Id || '',
                address: loc.Address || 'Unknown address'
            }));
        } catch (error) {
            console.error('Error fetching trip locations:', error);
            return [];
        }
    },

    // Get available trips with filters
    searchAvailableTrips: async (params: {
        origin?: string;
        destination?: string;
        date?: string;
    }): Promise<AvailableTripDto[]> => {
        try {
            const response = await axiosInstance.get('/PassengerTrip/available/search', { params });
            return response.data.map((trip: any) => ({
                id: trip.Id || '',
                origin: trip.Origin || 'Unknown origin',
                destination: trip.Destination || 'Unknown destination',
                departureTime: trip.DepartureTime || new Date().toISOString(),
                availableSeats: trip.AvailableSeats || 0,
                status: trip.Status || 'Unknown',
                driverName: trip.DriverName || 'Unknown driver'
            }));
        } catch (error) {
            console.error('Error searching trips:', error);
            return [];
        }
    }
};