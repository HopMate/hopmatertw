import React, { useEffect, useRef, useState } from 'react';
import { axiosInstance } from '../axiosConfig';
import { useNavigate, Link } from 'react-router-dom';
import Layout from '../components/Layout';

interface UserInfo {
    username: string;
    email?: string;
}

const Dashboard: React.FC = () => {
    const [user, setUser] = useState<UserInfo | null>(null);
    const navigate = useNavigate();

    const [showDriverMenu, setShowDriverMenu] = useState(false);
    const [showPassengerMenu, setShowPassengerMenu] = useState(false);

    const driverMenuRef = useRef<HTMLDivElement>(null);
    const passengerMenuRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        axiosInstance.get('/dashboard/userinfo')
            .then((response) => setUser(response.data))
            .catch((error) => console.error('Erro ao buscar dados protegidos:', error));
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    // Close dropdowns when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                driverMenuRef.current && !driverMenuRef.current.contains(event.target as Node)
            ) {
                setShowDriverMenu(false);
            }
            if (
                passengerMenuRef.current && !passengerMenuRef.current.contains(event.target as Node)
            ) {
                setShowPassengerMenu(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <Layout>
            <main className="p-6 flex flex-col items-center justify-center text-center">
                <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-4">Dashboard do Projeto</h1>
                <p className="text-lg text-gray-600 dark:text-gray-300 max-w-xl">
                    Esta é a interface principal do projeto onde podes navegar pelas APIs, ver dados protegidos e interagir com as funcionalidades do sistema.
                </p>

                {user && (
                    <div className="mt-6 bg-gray-100 dark:bg-gray-800 p-4 rounded shadow w-full max-w-2xl">
                        <h2 className="text-lg font-semibold text-gray-700 dark:text-white mb-2">Informação do Utilizador</h2>
                        <pre className="bg-gray-800 text-white p-4 rounded-lg text-left">
                            {JSON.stringify(user, null, 2)}
                        </pre>
                    </div>
                )}
            </main>
        </Layout>
    );
};

export default Dashboard;
