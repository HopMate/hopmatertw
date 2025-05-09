// In your types.ts
export interface Trip {
    id: string;
    origin: string;
    destination: string;
    departureTime: string;
    availableSeats: number;
    status: string;
    requestCount: number;
}

export interface PassengerRequest {
    id: string;
    passengerId: string;
    passengerName: string;
    tripId: string;
    locationId: string;
    pickupLocation: string;
    requestStatusId: number;
    requestStatus: string;
    requestDate: string;
    reason: string | null;
}

export interface AcceptRequestPayload {
    requestId: string;
}

export interface RejectRequestPayload {
    requestId: string;
    reason: string;
}

// For passenger to view available trips
export interface AvailableTripDto {
    id: string;
    origin: string;
    destination: string;
    departureTime: string;
    availableSeats: number;
    status: string;
    driverName: string;
}

// For checking trip availability before requesting
export interface TripAvailabilityDto {
    tripId: string;
    hasAvailableSeats: boolean;
    availableSeats: number;
    totalSeats: number;
}

// For creating a participation request
export interface CreateParticipationRequestDto {
    tripId: string;
    locationId: string;
    postalCode: string;
}

// For joining a waiting list
export interface JoinWaitingListDto {
    tripId: string;
    locationId: string;
    postalCode: string;
}

// For displaying passenger trip participation
export interface PassengerTripDto {
    id: string;
    passengerId: string;
    passengerName?: string;
    tripId: string;
    locationId: string;
    pickupLocation?: string;
    requestStatusId: number;
    requestStatus?: string;
    requestDate: string;
    reason?: string | null;
    previousStatus?: number; // Add this to track if came from waiting list
    queuePosition?: number; // Add this for waiting list position
    trip?: {
        origin: string;
        destination: string;
        departureTime: string;
        driverName?: string;
        availableSeats?: number;
    };
}

export interface LocationDto {
    id: string;
    address: string;
}