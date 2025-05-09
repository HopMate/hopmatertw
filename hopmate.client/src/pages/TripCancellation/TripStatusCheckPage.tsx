import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import { axiosInstance } from '../../axiosConfig';
import Layout from '../../components/Layout';

interface TripSimilarityRequestDto {
    dateDeparture: string;
    dateArrival: string;
    postalOrigin: string;
    postalDestination: string;
}

interface TripResult {
    id: string;
    driver: { name: string };
    vehicle: { brand: string; model: string; plate: string };
    dtDeparture: string;
    dtArrival: string;
}

const TripStatusCheckPage: React.FC = () => {
    const { id } = useParams();
    const [similarDto, setSimilarDto] = useState<TripSimilarityRequestDto | null>(null);
    const [results, setResults] = useState<TripResult[]>([]);
    const [statusMessage, setStatusMessage] = useState('');

    const checkStatus = async () => {
        try {
            const response = await axiosInstance.get(`/trip/status/${id}`);
            const status = response.data.status;

            if (status === 4) {
                setStatusMessage('A viagem já foi cancelada.');

                // Tenta buscar o DTO usado para procurar semelhantes
                const tripResponse = await axiosInstance.get(`/trip/detailsimilarity/${id}`);
                setSimilarDto(tripResponse.data); // ← Este endpoint deve devolver o TripSimilarityRequestDto
            } else {
                setStatusMessage('A viagem ainda está ativa.');
            }
        } catch {
            alert('Erro ao verificar o estado da viagem');
        }
    };

    const handleSearchSimilar = async () => {
        if (!similarDto) return;
        try {
            const response = await axiosInstance.post('/trip/searchsimilar', similarDto);
            setResults(response.data);
        } catch {
            alert('Erro ao procurar viagens semelhantes');
        }
    };

    return (
        <Layout>
            <div className="p-4">
                <h2 className="text-xl font-bold mb-4">Estado da Viagem</h2>

                <button
                    onClick={checkStatus}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                    Verificar estado da viagem
                </button>

                {statusMessage && <p className="mt-4">{statusMessage}</p>}

                {similarDto && (
                    <div className="mt-4">
                        <button
                            onClick={handleSearchSimilar}
                            className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                        >
                            Procurar viagens semelhantes
                        </button>
                    </div>
                )}

                {results.length > 0 && (
                    <div className="mt-6">
                        <h3 className="text-lg font-semibold mb-2">Viagens semelhantes encontradas:</h3>
                        {results.map((trip) => (
                            <div key={trip.id} className="bg-gray-100 p-3 rounded mb-2">
                                <p><strong>Motorista:</strong> {trip.driver.name}</p>
                                <p><strong>Veículo:</strong> {trip.vehicle.brand} {trip.vehicle.model} ({trip.vehicle.plate})</p>
                                <p><strong>Partida:</strong> {new Date(trip.dtDeparture).toLocaleString()}</p>
                                <p><strong>Chegada:</strong> {new Date(trip.dtArrival).toLocaleString()}</p>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </Layout>
    );
};

export default TripStatusCheckPage;
