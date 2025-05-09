import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { axiosInstance } from '../axiosConfig';
import { AxiosError } from 'axios';

interface ErrorMessages {
    [key: string]: string;
}

interface RegisterErrorResponse {
    errors?: ErrorMessages;
    message?: string;
}

const Register: React.FC = () => {
    const [username, setUsername] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [dateOfBirth, setDateOfBirth] = useState<string>('');
    const [name, setName] = useState<string>('');
    const [errorMessages, setErrorMessages] = useState<ErrorMessages>({});
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            navigate('/dashboard');
        }
    }, [navigate]);

    const handleLoginRequest = () => {
        navigate('/login');
    };

    const handleRegister = async (event: React.FormEvent) => {
        event.preventDefault();

        const passwordRegex = /^(?=.*[A-Z])(?=.*\W).{8,}$/;
        if (!passwordRegex.test(password)) {
            setErrorMessages({
                password: 'Password must be at least 8 characters long, include at least one uppercase letter and one symbol.',
            });
            return;
        }

        try {
            await axiosInstance.post('/auth/register', {
                username,
                email,
                password,
                dateOfBirth: new Date(dateOfBirth).toISOString().split('T')[0],
                name,
                hasDrivingLicense: false,
            });
            navigate('/login');
        } catch (error: unknown) {
            const axiosError = error as AxiosError<RegisterErrorResponse>;
            const errors = axiosError.response?.data?.errors || {};
            const formattedErrors: ErrorMessages = {};

            if (errors.username) formattedErrors.username = errors.username;
            if (errors.email) formattedErrors.email = errors.email;
            if (errors.password) formattedErrors.password = errors.password;
            if (errors.dateOfBirth) formattedErrors.dateOfBirth = errors.dateOfBirth;
            if (axiosError.response?.data?.message) formattedErrors.general = axiosError.response.data.message;

            setErrorMessages(formattedErrors);
        }
    };

    return (
        <section className="bg-gray-50 dark:bg-gray-900">
            <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0">
                <a href="#" className="flex items-center mb-6 text-2xl font-semibold text-gray-900 dark:text-white">Hopmate</a>
                <div className="w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
                            Create a new account
                        </h1>
                        <form onSubmit={handleRegister} className="space-y-4 md:space-y-6">
                            <div>
                                <label htmlFor="name" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Full Name</label>
                                <input
                                    type="text"
                                    name="name"
                                    id="name"
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                    className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    placeholder="Full Name"
                                    required
                                />
                                {errorMessages.name && <p className="text-sm text-red-500 mt-1">{errorMessages.name}</p>}
                            </div>
                            <div>
                                <label htmlFor="username" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Username</label>
                                <input
                                    type="text"
                                    name="username"
                                    id="username"
                                    value={username}
                                    onChange={(e) => setUsername(e.target.value)}
                                    className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    placeholder="Username"
                                    required
                                />
                                {errorMessages.username && <p className="text-sm text-red-500 mt-1">{errorMessages.username}</p>}
                            </div>
                            <div>
                                <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Email</label>
                                <input
                                    type="email"
                                    name="email"
                                    id="email"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    placeholder="Email"
                                    required
                                />
                                {errorMessages.email && <p className="text-sm text-red-500 mt-1">{errorMessages.email}</p>}
                            </div>
                            <div>
                                <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Password</label>
                                <input
                                    type="password"
                                    name="password"
                                    id="password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    placeholder="Password"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    required
                                />
                                {errorMessages.password && <p className="text-sm text-red-500 mt-1">{errorMessages.password}</p>}
                            </div>
                            <div>
                                <label htmlFor="dateOfBirth" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Date of Birth</label>
                                <input
                                    type="date"
                                    name="dateOfBirth"
                                    id="dateOfBirth"
                                    value={dateOfBirth}
                                    onChange={(e) => setDateOfBirth(e.target.value)}
                                    className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    required
                                />
                                {errorMessages.dateOfBirth && <p className="text-sm text-red-500 mt-1">{errorMessages.dateOfBirth}</p>}
                            </div>
                            {errorMessages.general && <p className="text-sm text-red-500 mt-1">{errorMessages.general}</p>}
                            <button
                                type="submit"
                                className="w-full bg-blue-600 text-white font-semibold rounded-lg px-5 py-2.5 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            >
                                Sign up
                            </button>
                        </form>
                        <p className="text-sm font-light text-gray-500 dark:text-gray-400">
                           Already have an account?{' '}
                            <button
                                type="button"
                                onClick={handleLoginRequest}
                                className="font-medium text-blue-600 hover:underline dark:text-blue-500"
                            >
                                Log in
                            </button>
                        </p>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default Register;
