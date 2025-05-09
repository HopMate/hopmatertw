import React, { useEffect, useRef, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { axiosInstance } from '../axiosConfig';
import { HiChevronDown } from 'react-icons/hi';

interface UserInfo {
    username: string;
    email?: string;
}

const Navbar: React.FC = () => {
    const [user, setUser] = useState<UserInfo | null>(null);
    const navigate = useNavigate();

    const [showDriverMenu, setShowDriverMenu] = useState(false);
    const [showPassengerMenu, setShowPassengerMenu] = useState(false);

    const driverMenuRef = useRef<HTMLDivElement>(null);
    const passengerMenuRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        axiosInstance.get('/dashboard/userinfo')
            .then((response) => setUser(response.data))
            .catch((error) => console.error('Error fetching user info:', error));
    }, []);

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
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <nav className="bg-white dark:bg-gray-800 shadow px-6 py-4 flex justify-between items-center">
            <div className="flex space-x-4 items-center text-blue-600 font-semibold">
                <Link to="/" className="hover:underline">Home</Link>
                <Link to="/api/trips" className="hover:underline">Trips API</Link>
                <Link to="/api/users" className="hover:underline">Users API</Link>
                <Link to="/api/vehicles" className="hover:underline">Vehicles API</Link>

                {/* Driver Dropdown */}
                <div className="relative" ref={driverMenuRef}>
                    <button
                        onClick={() => setShowDriverMenu(!showDriverMenu)}
                        className="hover:underline flex items-center gap-1"
                    >
                        Driver <HiChevronDown className="w-4 h-4" />
                    </button>
                    {showDriverMenu && (
                        <div className="absolute mt-2 w-48 bg-white text-black rounded shadow-lg z-10">
                            <Link to="/driver/trips" className="block px-4 py-2 hover:bg-gray-100">My Trips</Link>
                            <Link to="/driver/requests" className="block px-4 py-2 hover:bg-gray-100">Trip Requests</Link>
                        </div>
                    )}
                </div>

                {/* Passenger Dropdown */}
                <div className="relative" ref={passengerMenuRef}>
                    <button
                        onClick={() => setShowPassengerMenu(!showPassengerMenu)}
                        className="hover:underline flex items-center gap-1"
                    >
                        Passenger <HiChevronDown className="w-4 h-4" />
                    </button>
                    {showPassengerMenu && (
                        <div className="absolute mt-2 w-56 bg-white text-black rounded shadow-lg z-10">
                            <Link to="/passenger/my-trips" className="block px-4 py-2 hover:bg-gray-100">My Trips</Link>
                            <Link to="/passenger/available-trips" className="block px-4 py-2 hover:bg-gray-100">Available Trips</Link>
                        </div>
                    )}
                </div>
            </div>

            <div className="flex items-center space-x-4">
                {user && (
                    <span className="text-gray-700 dark:text-gray-300">
                        Olá, <strong>{user.username}</strong>
                    </span>
                )}
                <button
                    onClick={handleLogout}
                    className="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700"
                >
                    Logout
                </button>
            </div>
        </nav>
    );
};

export default Navbar;
