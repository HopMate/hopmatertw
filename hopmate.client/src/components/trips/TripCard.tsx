// client/src/components/trips/TripCard.tsx
import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Trip } from '../../types';
import Badge from '../ui/Badge';
import Button from '../ui/Button';
import { axiosInstance } from '../../axiosConfig';

interface TripCardProps {
    trip: Trip;
    pendingRequestsCount?: number;
    onTripCancelled?: (tripId: string) => void;
}

const TripCard: React.FC<TripCardProps> = ({ trip: initialTrip, pendingRequestsCount = 0, onTripCancelled }) => {
    const userId = localStorage.getItem('userId');
    // Usar estado local para permitir atualizações na UI sem recarregar a página
    const [trip, setTrip] = useState<Trip>(initialTrip);
    const [isLoading, setIsLoading] = useState(false);

    // Constantes para melhorar a legibilidade
    const TRIP_STATUS_CANCELLED = 4;
    const STATUS_MAP: Record<number, string> = {
        1: 'Planned',
        2: 'In Progress',
        3: 'Completed',
        4: 'Cancelled',
        // Adicione outros status conforme necessário
    };

    const handleCancelTrip = async (tripId: string) => {
        if (!userId) {
            alert('Utilizador não autenticado.');
            return;
        }

        const confirmed = window.confirm('Tens a certeza que queres cancelar esta viagem?');
        if (!confirmed) return;

        setIsLoading(true);

        try {
            await axiosInstance.post(`/trip/cancel/${tripId}`, { idDriver: userId }, {
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            // Atualizar o estado local para refletir o cancelamento
            setTrip({
                ...trip,
                status: 'Cancelled',
            });

            alert('Viagem cancelada com sucesso!');

            // Notificar o componente pai sobre o cancelamento, se o callback existir
            if (onTripCancelled) {
                onTripCancelled(tripId);
            }

        } catch (error) {
            console.error('Erro ao cancelar viagem:', error);
            alert('Erro ao cancelar viagem.');
        } finally {
            setIsLoading(false);
        }
    };

    // Better date handling with fallback
    const formatDate = (dateString: string | undefined) => {
        if (!dateString) return 'Data não disponível';
        try {
            const date = new Date(dateString);
            // Check if date is valid
            if (isNaN(date.getTime())) {
                return 'Data inválida';
            }
            return date.toLocaleString();
        } catch (error) {
            console.error('Error formatting date:', error);
            return 'Erro de data';
        }
    };

    // Verificar se a viagem está cancelada
    const isCancelled = trip.status === TRIP_STATUS_CANCELLED.toString() || trip.status === 'Cancelled';

    // Obter o nome do status a partir do mapa ou usar o valor atual
    const displayStatus = trip.status ? STATUS_MAP[Number(trip.status)] || trip.status : trip.status;

    return (
        <div className="bg-white shadow rounded-lg p-4 mb-4">
            <div className="flex justify-between items-start mb-2">
                <h3 className="text-lg font-medium">
                    {trip.origin || 'Origem desconhecida'} → {trip.destination || 'Destino desconhecido'}
                </h3>
                {/* Mostrar badge de assentos apenas se não estiver cancelada */}
                {!isCancelled && (
                    <Badge variant={(trip.availableSeats > 0) ? 'success' : 'danger'}>
                        {typeof trip.availableSeats === 'number' ? trip.availableSeats : 0} lugares disponíveis
                    </Badge>
                )}
            </div>
            <div className="text-sm text-gray-600 mb-4">
                <p>Partida: {formatDate(trip.departureTime)}</p>
                <p>Status:
                    <span className={isCancelled ? 'text-red-600 font-medium' : ''}>
                        {' ' + displayStatus || 'Desconhecido'}
                    </span>
                </p>
            </div>
            <div className="flex justify-between items-center">
                {!isCancelled && pendingRequestsCount > 0 && (
                    <Badge variant="warning" className="mr-2">
                        {pendingRequestsCount} pedidos pendentes
                    </Badge>
                )}
                <div className="space-x-2">
                    {!isCancelled && (
                        <Link to={`/driver/trips/${trip.id}/requests`}>
                            <Button variant="primary" disabled={isLoading}>
                                Gerir Pedidos
                            </Button>
                        </Link>
                    )}
                </div>
                {/* Mostrar botão cancelar apenas se a viagem não estiver cancelada */}
                {!isCancelled && (
                    <Button
                        variant="danger"
                        onClick={() => handleCancelTrip(trip.id)}
                        disabled={isLoading}
                        className="text-red-600 hover:underline"
                    >
                        {isLoading ? 'A cancelar...' : 'Cancelar'}
                    </Button>
                )}
            </div>
        </div>
    );
};

export default TripCard;