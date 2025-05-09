// client/src/components/trips/RequestCard.tsx
import React, { useState } from 'react';
import { PassengerRequest } from '../../types';
import Badge from '../ui/Badge';
import Button from '../ui/Button';
import { tripService } from '../../services/tripService';

interface RequestCardProps {
    request: PassengerRequest;
    onStatusChange?: () => void;
}

const RequestCard: React.FC<RequestCardProps> = ({ request, onStatusChange }) => {
    const [isLoading, setIsLoading] = useState(false);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectionReason, setRejectionReason] = useState('');

    const getStatusBadgeVariant = () => {
        switch (request.requestStatus.toLowerCase()) {
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

    const handleAccept = async () => {
        try {
            setIsLoading(true);
            await tripService.acceptRequest({ requestId: request.id });
            if (onStatusChange) onStatusChange();
        } catch (error) {
            console.error('Error accepting request:', error);
            alert('Failed to accept request. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleReject = async () => {
        if (!rejectionReason.trim()) {
            alert('Please provide a reason for rejection');
            return;
        }

        try {
            setIsLoading(true);
            await tripService.rejectRequest({
                requestId: request.id,
                reason: rejectionReason
            });
            if (onStatusChange) onStatusChange();
            setShowRejectForm(false);
            setRejectionReason('');
        } catch (error) {
            console.error('Error rejecting request:', error);
            alert('Failed to reject request. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    const requestDate = new Date(request.requestDate).toLocaleString();

    return (
        <div className="bg-white shadow rounded-lg p-4 mb-4">
            <div className="flex justify-between items-start mb-2">
                <h3 className="text-lg font-medium">{request.passengerName}</h3>
                <Badge variant={getStatusBadgeVariant()}>
                    {request.requestStatus}
                </Badge>
            </div>

            <div className="text-sm text-gray-600 mb-4">
                <p>Pickup Location: {request.pickupLocation}</p>
                <p>Request Date: {requestDate}</p>
                {request.reason && <p>Reason: {request.reason}</p>}
            </div>

            {request.requestStatus.toLowerCase() === 'pending' && request.previousStatus === 4 && (
                <div className="mb-3 p-2 bg-blue-50 text-blue-800 text-sm rounded">
                    This request was moved from the waiting list due to seat availability.
                </div>
            )}

            {request.requestStatus.toLowerCase() === 'pending' && (
                <div className="space-y-2">
                    {!showRejectForm ? (
                        <div className="flex space-x-2">
                            <Button
                                variant="success"
                                onClick={handleAccept}
                                disabled={isLoading}
                            >
                                Accept
                            </Button>
                            <Button
                                variant="danger"
                                onClick={() => setShowRejectForm(true)}
                                disabled={isLoading}
                            >
                                Reject
                            </Button>
                        </div>
                    ) : (
                        <div className="space-y-2">
                            <textarea
                                className="w-full p-2 border rounded"
                                rows={2}
                                placeholder="Reason for rejection"
                                value={rejectionReason}
                                onChange={(e) => setRejectionReason(e.target.value)}
                            />
                            <div className="flex space-x-2">
                                <Button
                                    variant="danger"
                                    onClick={handleReject}
                                    disabled={isLoading}
                                >
                                    Confirm Rejection
                                </Button>
                                <Button
                                    variant="secondary"
                                    onClick={() => setShowRejectForm(false)}
                                    disabled={isLoading}
                                >
                                    Cancel
                                </Button>
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default RequestCard;