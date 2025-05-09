/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import { axiosInstance } from '../../axiosConfig';
import Layout from '../../components/Layout';
import axios from 'axios';

interface TripSimilarityRequestDto {
    dateDeparture: string;
    dateArrival: string;
    postalOrigin: string;
    postalDestination: string;
}

interface TripDto {
    id: string;
    dtDeparture: string;
    dtArrival: string;
}

const CancelTripPage: React.FC = () => {
    const { id } = useParams();
    const [step, setStep] = useState<'initial' | 'confirmSearch' | 'showResult'>('initial');
    const [similarityDto, setSimilarityDto] = useState<TripSimilarityRequestDto | null>(null);
    const [similarTrip, setSimilarTrip] = useState<TripDto | null>(null);
    const [isCooldown, setIsCooldown] = useState(false);

    const handleCancelTrip = async () => {
        if (isCooldown) return;
        setIsCooldown(true);
        setTimeout(() => setIsCooldown(false), 5000);

        try {
            const response = await axiosInstance.post(`/trip/cancel/${id}`);
            if (typeof response.data === 'string') {
                alert(response.data);
            } else {
                setSimilarityDto(response.data);
                setStep('confirmSearch');
            }
        } catch (error: any) {
            if (axios.isAxiosError(error) && error.response) {
                alert(error.response.data || 'Erro ao cancelar viagem');
            } else {
                alert('Erro inesperado.');
            }
        }
    };

    const handleSearchSimilar = async () => {
        if (!similarityDto) return;
        try {
            const response = await axiosInstance.post('/trip/searchsimilar', similarityDto);
            setSimilarTrip(response.data);
            setStep('showResult');
        } catch {
            alert('Erro ao procurar viagem semelhante');
        }
    };

    const resetFlow = () => {
        setStep('initial');
        setSimilarityDto(null);
        setSimilarTrip(null);
    };

    return (
        <Layout>
            <div className="p-4 max-w-xl mx-auto">
                {step === 'initial' && (
                    <div>
                        <h2 className="text-xl font-bold mb-4">Cancelar viagem</h2>
                        <button
                            onClick={handleCancelTrip}
                            disabled={isCooldown}
                            className={`px-4 py-2 rounded text-white transition ${isCooldown ? 'bg-gray-400 cursor-not-allowed' : 'bg-red-600 hover:bg-red-700'
                                }`}
                        >
                            {isCooldown ? 'Aguarde...' : 'Confirmar cancelamento'}
                        </button>
                    </div>
                )}

                {step === 'confirmSearch' && (
                    <div>
                        <p className="mb-4">Viagem cancelada com sucesso. Queres procurar uma semelhante?</p>
                        <button
                            onClick={handleSearchSimilar}
                            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 mr-2"
                        >
                            Sim
                        </button>
                        <button
                            onClick={resetFlow}
                            className="bg-gray-400 text-white px-4 py-2 rounded hover:bg-gray-500"
                        >
                            Não
                        </button>
                    </div>
                )}

                {step === 'showResult' && (
                    <div>
                        <h3 className="text-lg font-semibold mb-2">Viagem semelhante encontrada:</h3>
                        {similarTrip ? (
                            <div className="bg-gray-100 p-4 rounded shadow">
                                <p><strong>ID:</strong> {similarTrip.id}</p>
                                <p><strong>Partida:</strong> {new Date(similarTrip.dtDeparture).toLocaleString()}</p>
                                <p><strong>Chegada:</strong> {new Date(similarTrip.dtArrival).toLocaleString()}</p>
                            </div>
                        ) : (
                            <p>Nenhuma viagem semelhante foi encontrada.</p>
                        )}
                        <button
                            onClick={resetFlow}
                            className="mt-4 bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
                        >
                            Voltar
                        </button>
                    </div>
                )}
            </div>
        </Layout>
    );
};

export default CancelTripPage;
